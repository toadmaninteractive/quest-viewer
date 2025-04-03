using QuestGraph.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuestViewer.ServerForExternalClient
{
    public class HttpServer : ServerForExternalClient, IServerForExternalClient
    {
        private class ExternalClient
        {
            public readonly WebSocket WebSocket;
            public readonly int ConnectionNumber;
            public Queue<string> ResponseQueue = new Queue<string>();
            public bool IsActive => WebSocket != null && WebSocket.State == WebSocketState.Open;

            public ExternalClient(WebSocket webSocket, int connectionNumber)
            {
                WebSocket = webSocket;
                ConnectionNumber = connectionNumber;
            }

            public async Task CloseAsync()
            {
                await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                WebSocket.Dispose();
            }

            public void Abort()
            {
                WebSocket.Abort();
                WebSocket.Dispose();
            }
        }

        public event Action<int> OnActiveConnectionCountChanged;

        private List<ExternalClient> externalClients = new List<ExternalClient>();
        private HttpListener listener = new HttpListener();
        private string loggingPrefix = "HttpServer";
        
        public HttpServer(IInterProcessCommunication interProcessCommunicationModel) : base(interProcessCommunicationModel)
        {
            listener.Prefixes.Add("http://localhost:55555/ws/");
        }

        public void Start()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine($"{loggingPrefix} is not supportted");
                return;
            }

            shouldStop = false;
            externalClients.Clear();
            listener.Start();
            logger.Debug($"{loggingPrefix} has been started");
        }

        public async Task StartListennig()
        {
            logger.Debug($"{loggingPrefix} start listenning...");
            while (!shouldStop)
            {
                try
                {
                    var httpListenerContext = await listener.GetContextAsync();
                    logger.Debug($"{loggingPrefix} has http Listener Context");

                    if (!httpListenerContext.Request.IsWebSocketRequest)
                    {
                        httpListenerContext.Response.StatusCode = 400;
                        httpListenerContext.Response.Close();
                        continue;
                    }

                    var webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);
                    logger.Debug($"{loggingPrefix} has http Socket Context");
                    RunLoop(webSocketContext.WebSocket, externalClients.Count).Track();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{loggingPrefix} listennig exception: {ex.GetBaseException().Message}{Environment.NewLine}{Environment.StackTrace}");
                }
            }
        }

        private async Task RunLoop(WebSocket webSocket, int connectionNumber)
        {
            var externalClient = new ExternalClient(webSocket, connectionNumber);
            externalClients.Add(externalClient);
            OnActiveConnectionCountChanged?.Invoke(externalClients.Count);

            await Task.WhenAll(RequestListeningLoop(externalClient), ResponseSendingLoop(externalClient));

            logger.Debug($"{loggingPrefix} [connection {externalClient.ConnectionNumber}] has stoped");
            await externalClient.CloseAsync();
            externalClients.Remove(externalClient);
            OnActiveConnectionCountChanged?.Invoke(externalClients.Count);
        }

        private async Task RequestListeningLoop(ExternalClient externalClient)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;
            var completeMessage = new StringBuilder();

            try
            {
                while (externalClient.WebSocket.State == WebSocketState.Open)
                {
                    completeMessage.Clear();

                    #region parsing
                    do
                    {
                        try
                        {
                            result = await externalClient.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            var messagePart = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            completeMessage.Append(messagePart);
                        }
                        catch (WebSocketException ex)
                        {
                            var baseException = ex.GetBaseException();
                            if (baseException is HttpListenerException httpListenerException && httpListenerException.ErrorCode == 995)
                                logger.Info($"{loggingPrefix} [connection {externalClient.ConnectionNumber}] has been dropped ({baseException.Message})");
                            else
                                logger.Error(ex, $"{loggingPrefix} [connection {externalClient.ConnectionNumber}] lost connection with external client: {baseException.Message}{Environment.NewLine}{Environment.StackTrace}");
                            return;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, $"{loggingPrefix} [connection {externalClient.ConnectionNumber}] {ex.GetBaseException().Message}{Environment.NewLine}{Environment.StackTrace}");
                            return;
                        }
                    } while (!result.EndOfMessage);
                    #endregion

                    string requestText = completeMessage.ToString();
                    logger.Debug($"{loggingPrefix} [connection {externalClient.ConnectionNumber}] has new requests source: {requestText}");

                    if (HandleRequest(requestText, out string responceText))
                        externalClient.ResponseQueue.Enqueue(responceText);
                }
            }
            finally
            {
                logger.Debug($"{loggingPrefix} [connection {externalClient.ConnectionNumber}] has stoped Listening Loop. IsActive status: {externalClient.IsActive}");
            }
        }

        private async Task ResponseSendingLoop(ExternalClient externalClient)
        {
            while (externalClient.WebSocket.State == WebSocketState.Open)
            {
                while (externalClient.ResponseQueue.Count > 0)
                {
                    var receivedText = externalClient.ResponseQueue.Dequeue();
                    var bytes = Encoding.UTF8.GetBytes(receivedText);
                    await externalClient.WebSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }

                await Task.Delay(100);
            }

            logger.Debug($"{loggingPrefix} [connection {externalClient.ConnectionNumber}] has stoped Sending Loop. IsActive status: {externalClient.IsActive}");
        }

        public void Stop()
        {
            shouldStop = true;
            listener.Stop();
            listener.Close();
            foreach (var externalClient in externalClients)
                externalClient.Abort();
            logger.Debug($"{loggingPrefix} has been stopped");
        }

        protected override void SendMessageHandler(BlockStateUpdateArg updateArg)
        {
            var message = BlockStateSerialize(updateArg);
            logger.Debug($"{loggingPrefix} is tring to send message to external client: {message}");
            try
            {
                foreach (var externalClient in externalClients)
                {
                    externalClient.ResponseQueue.Enqueue(message);
                    logger.Debug($"{loggingPrefix} has to sent Message to external client successfully");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{loggingPrefix} has failed to send message to external client: {ex.GetBaseException().Message}{Environment.NewLine}{Environment.StackTrace}");
            }
        }
    }
}