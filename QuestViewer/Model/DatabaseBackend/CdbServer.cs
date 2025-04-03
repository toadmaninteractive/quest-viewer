using CouchDB.Api;
using Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuestViewer
{
    public class CdbServer
    {
        private static readonly SocketsHttpHandler HandlerInstance = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(3) };
        private HttpClient httpClient;
        private ICredentials credentials;
        private string serverBaseUrl;

        public CdbServer(Uri url, string username, byte[] password)
        {
            httpClient = new HttpClient(HandlerInstance, disposeHandler: false);
            if (username != null)
            {
                var basicAuthBytes = Encoding.UTF8.GetBytes($"{username}:{Encoding.Unicode.GetString(ProtectedData.Unprotect(password, null, DataProtectionScope.CurrentUser))}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(basicAuthBytes));

                credentials = new NetworkCredential(username, Encoding.Unicode.GetString(ProtectedData.Unprotect(password, null, DataProtectionScope.CurrentUser)));
            }
            httpClient.BaseAddress = url;
            serverBaseUrl = url.ToString();
        }

        public async Task<IReadOnlyCollection<string>> GetDatabaseNamesAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync("_all_dbs", cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.GetJsonResponseAsync(cancellationToken).ConfigureAwait(false);
            return result.AsArray.Select(db => db.AsString).ToArray();
        }

        public async Task<CdbServerResult<string[]>> GetQuestGraphDocumentsAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"{httpClient.BaseAddress}/{databaseName}/_design/dialogs/_view/dialogs", cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.GetJsonResponseAsync(cancellationToken).ConfigureAwait(false);
                var documents = result.AsObject["rows"].AsArray.Select(db => db.AsObject["id"].AsString).ToArray();
                return new CdbServerResult<string[]>(documents, true);
            }
            else
                return new CdbServerResult<string[]>(null, false);
        }

        public async Task<ImmutableJson> GetAllDocumentsAsync(
            string databaseName,
            bool conflicts = false,
            bool descending = false,
            string endKey = null,
            string endKeyDocId = null,
            bool includeDocs = false,
            bool inclusiveEnd = true,
            string key = null,
            int? limit = null,
            int skip = 0,
            string stale = null,
            string startKey = null,
            string startKeyDocId = null,
            bool updateSeq = false,
            CancellationToken cancellationToken = default)
        {
            var url = new CdbQueryBuilder(databaseName, "_all_docs")
                .AppendBoolean("conflicts", conflicts, false)
                .AppendBoolean("descending", descending, false)
                .AppendString("endkey", endKey)
                .AppendString("endkey_docid", endKeyDocId)
                .AppendBoolean("include_docs", includeDocs, false)
                .AppendBoolean("inclusive_end", inclusiveEnd, false)
                .AppendString("key", key)
                .AppendNumber("limit", limit)
                .AppendNumber("skip", skip, 0)
                .AppendString("stale", stale)
                .AppendString("startkey", startKey)
                .AppendString("startkey_docid", startKeyDocId)
                .AppendBoolean("update_seq", updateSeq, false)
                .ToUri();
            using var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.GetJsonResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<ImmutableJson> GetDocumentAsync(
            string databaseName,
            string docId,
            bool attachments = false,
            bool attEncodingInfo = false,
            IEnumerable<string> attsSince = null,
            bool conflicts = false,
            bool deletedConflicts = false,
            bool latest = false,
            bool localSeq = false,
            bool meta = false,
            IEnumerable<string> openRevs = null,
            bool openRevsAll = false,
            string rev = null,
            bool revs = false,
            bool revsInfo = false,
            CancellationToken cancellationToken = default)
        {
            var url = new CdbQueryBuilder(databaseName, docId)
                .AppendBoolean("attachments", attachments, false)
                .AppendBoolean("att_encoding_info", attEncodingInfo, false)
                .AppendStringArray("atts_since", attsSince)
                .AppendBoolean("conflicts", conflicts, false)
                .AppendBoolean("deleted_conflicts", deletedConflicts, false)
                .AppendBoolean("latest", latest, false)
                .AppendBoolean("local_seq", localSeq, false)
                .AppendBoolean("meta", meta, false)
                .AppendStringArray("open_revs", openRevs)
                .AppendString("open_revs", openRevsAll ? "all" : null)
                .AppendString("rev", rev)
                .AppendBoolean("revs", revs, false)
                .AppendBoolean("revs_info", revsInfo, false)
                .ToUri();

            using var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.GetJsonResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        #region Cdb server listening

        public async Task<string> GetLastSinceSeq(string databaseName, CancellationToken cancellationToken = default)
        {
            string url = databaseName + "/_changes?heartbeat=30000&include_docs=false";
            var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            var responseJson = await response.Content.GetJsonResponseAsync(cancellationToken);
            return responseJson["last_seq"].AsString;
        }

        public IObservable<BackendChange> ListenForChanges(string databaseName, string sinceSeq)
        {
            return Observable.Create<BackendChange>((observer, cancellationToken) => Task.Run(() => Listen(databaseName, sinceSeq, observer.OnNext, cancellationToken), cancellationToken));
        }

        void Listen(string databaseName, string sinceSeq, Action<BackendChange> callback, CancellationToken cancellationToken)
        {
            string url = serverBaseUrl + "/" + databaseName + "/_changes?feed=continuous&heartbeat=30000&include_docs=true&since=" + sinceSeq;
            var request = WebRequest.Create(url);
            request.Credentials = credentials;
            cancellationToken.Register(request.Abort);

            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    //responseStream.ReadTimeout = 35000;
                    using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            if (line.Length > 0)
                            {
                                var change = CdbChangeJsonSerializer.Instance.Deserialize(JsonParser.Parse(line));
                                callback(CdbToBackendChange(change));
                            }
                        }
                    }
                }
            }
        }

        private BackendChange CdbToBackendChange(CdbChange change)
        {
            if (change.Deleted)
                return BackendChange.Delete(SeqToString(change.Seq), change.Id);
            else
                return BackendChange.Update(SeqToString(change.Seq), change.Id, change.Doc?.AsObject, change.Changes.First().Rev);
        }

        private string SeqToString(ImmutableJson jsonSeq)
        {
            if (jsonSeq.IsString)
                return jsonSeq.AsString;
            else if (jsonSeq.IsInt)
                return jsonSeq.AsInt.ToString();
            else
                return jsonSeq.ToString();
        }

        #endregion
    }
}
