using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace QuestViewer
{
    public static class ChronosAPI
    {
        private static ChronosWebSocketClient instance;

        public static ChronosWebSocketClient Instance
        {
            get => instance;
        }

        public static void Initialization(Uri uri, string apiKey, string app, string component)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var assemblyVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = $"{assemblyVersion.FileMajorPart}.{assemblyVersion.FileMinorPart}.{assemblyVersion.FileBuildPart}";

            var revision = Utils.GetAppRevision();
            var config = Config.Instance;
            var branch = config.UpdateChannel.ToString().ToLower();

            instance = new ChronosWebSocketClient(uri, apiKey, app, component, branch, $"{version}.{revision}");
            instance.RunAsync().Track();
        }
    }

    public sealed class ChronosWebSocketClient
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Uri uri;
        private readonly string apiKey;
        private readonly string app;
        private readonly string component;
        private readonly string branch;
        private readonly string version;
        private readonly Channel<List<LogEntry>> channel;
        private bool chronosExceptionReported = false;

        public ChronosWebSocketClient(Uri uri, string apiKey, string app, string component, string branch, string version)
        {
            this.uri = uri;
            this.apiKey = apiKey;
            this.app = app;
            this.component = component;
            this.branch = branch;
            this.version = version;
            channel = Channel.CreateUnbounded<List<LogEntry>>();
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        using var webSocket = new ClientWebSocket();
                        webSocket.Options.SetRequestHeader("X-Api-Key", apiKey);
                        await webSocket.ConnectAsync(uri, ct).ConfigureAwait(false);
                        await SendLogsAsync(webSocket, ct).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        logger.Debug(exception, Texts.CodeErrors.ChronosLog);
                    }

                    await Task.Delay(30000, ct).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public ValueTask AppLogAsync(List<LogEntry> logs, CancellationToken ct = default)
        {
            return channel.Writer.WriteAsync(logs, ct);
        }

        private async Task SendLogsAsync(ClientWebSocket webSocket, CancellationToken ct)
        {
            int rpcId = 0;
            var buffer = new byte[4096];
            using var memoryStream = new MemoryStream();
            await foreach (var logs in channel.Reader.ReadAllAsync(ct).ConfigureAwait(false))
            {
                rpcId++;
                var message = new ServiceChronos.Log { App = app, Component = component, Branch = branch, Version = version, Logs = logs, RpcId = rpcId };
                var sendJson = ServiceChronosJsonSerializer.Log.Instance.Serialize(message);
                var bytes = Encoding.UTF8.GetBytes(sendJson.ToString());
                await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, ct);

                memoryStream.SetLength(0);
                bool received = false;
                while (!received)
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, ct).ConfigureAwait(false);
                    if (result.CloseStatus.HasValue)
                    {
                        logger.Debug(string.Format(Texts.CodeErrors.ChronosConnectionClosed, result.CloseStatus.Value, result.CloseStatusDescription));
                        return;
                    }
                    memoryStream.Write(buffer, 0, result.Count);
                    received = result.EndOfMessage;
                }

                var jsonString = Encoding.UTF8.GetString(memoryStream.GetBuffer().AsSpan(0, (int)memoryStream.Length));
                var recvJson = Json.JsonParser.Parse(jsonString);
                var response = ServiceChronosJsonSerializer.LogResponse.Instance.Deserialize(recvJson);
                if (!response.IsSuccess && !chronosExceptionReported)
                {
                    logger.Debug(response.Exception, Texts.CodeErrors.ChronosLog);
                    chronosExceptionReported = true;
                }
            }
        }
    }
}