using Analytics;
using NLog;
using Scylla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.Service
{
    public class ScyllaConnector
    {
        private const string baseAddress = "https://scylla.yourcompany.com/ingest/";
        private const string appCode = "qvdev";
        private const string apiKey = "TOPSECRET";
        private HttpClient httpClient;
        private ScyllaApi scyllaApi;
        private static ScyllaConnector instance;
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private ScyllaConnector()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            scyllaApi = new ScyllaApi(httpClient);
        }

        public static ScyllaConnector LoadInstance()
        {
            if (instance == null)
                instance = new ScyllaConnector();
            return instance;
        }

        public async Task SendSessionStart()
        {
            var config = Config.Instance;
            var revisionString = Utils.GetAppRevision();
            int.TryParse(revisionString, out int revision);
            var analyticEvent = new SessionStart()
            {
                Branch = config.UpdateChannel.ToString(),
                Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                InstallationId = GetInstallationId(),
                Revision = revision,
                Username = Config.UserNameFromCurrentConnection,
            };

            await SendEventAsync(analyticEvent);
        }


        private async Task SendEventAsync(AnalyticsEvent analyticEvent)
        {
#if DEBUG
            return;
#endif
#pragma warning disable CS0162 // Unreachable code detected
            try

            {
                var requestContent = new ScyllaEnvelope()
                {
                    Events = new List<AnalyticsEvent> { analyticEvent }
                };
                await scyllaApi.SendEventsAsync(requestContent, appCode, apiKey);
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.ErrorReportText());
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        private string GetInstallationId()
        {
            //Environment.MachineName is not safe operation
            try
            {
                return Utils.StringToMD5(Environment.MachineName);
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.ErrorReportText());
            }

            return Texts.Anonymous;
        }

        public void Dispose()
        {
            scyllaApi.Dispose();
        }
    }
}