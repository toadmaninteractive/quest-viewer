using NLog;
using QuestGraph.Core;
using QuestViewer.ExternalClientRequests;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.ServerForExternalClient
{
    public class TcpServer : ServerForExternalClient, IServerForExternalClient
    {
        public event Action<int> OnActiveConnectionCountChanged;

        private TcpListener tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), 55555);
        private List<TcpClient> connections = new List<TcpClient>();
        private Queue<string> enqueuedMessages = new Queue<string>();
        private ITcpRequestSplitter requestSplitter = new UnicodeSymbolSplitter();//UnicodeSymbolSplitter EmptySymbolSplitter

        public TcpServer(IInterProcessCommunication interProcessCommunicationModel) : base (interProcessCommunicationModel) { }

        public async Task StartListennig()
        {
            logger.Debug("Tcp server start listenning...");

            await Task.Run(async () =>
            {
                while (!shouldStop)
                {
                    try
                    {
                        if (tcpListener.Pending())
                        {
                            logger.Debug("Tcp server has new pending");
                            var client = tcpListener.AcceptTcpClient();
                            connections.Add(client);
                        }

                        foreach (var client in connections)
                        {
                            NetworkStream stream = client.GetStream();
                            StringBuilder completeMessage = new StringBuilder();
                            while (stream.DataAvailable)
                            {
                                byte[] buffer = new byte[512];
                                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                                string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                completeMessage.Append(receivedMessage);
                            }

                            if (completeMessage.Length > 0)
                            {
                                var requestText = completeMessage.ToString();
                                logger.Debug($"Tcp server has new requests source:  {requestText}");
                                foreach (var requestSource in requestSplitter.Split(requestText))
                                {
                                    if (HandleRequest(requestSource, out string responceText, requestSplitter.DividerSymbol))
                                        enqueuedMessages.Enqueue(responceText + requestSplitter.DividerSymbol);
                                }
                            }
                        }

                        while (enqueuedMessages.Count > 0)
                        {
                            string messageToSend = enqueuedMessages.Dequeue();
                            byte[] data = Encoding.ASCII.GetBytes(messageToSend);
                            byte[] buffer = new byte[65507];
                            int totalBytesSent = 0;

                            foreach (var client in connections)
                            {
                                Socket connectionSocket = client.GetStream().Socket;
                                int bytesLeftToSend = data.Length;
                                while (totalBytesSent < data.Length)
                                {
                                    int bytesToSend = Math.Min(buffer.Length, bytesLeftToSend);
                                    Buffer.BlockCopy(data, totalBytesSent, buffer, 0, bytesToSend);

                                    int bytesSent = connectionSocket.Send(buffer, 0, bytesToSend, SocketFlags.None);
                                    if (bytesSent == 0)
                                        break;

                                    totalBytesSent += bytesSent;
                                    bytesLeftToSend -= bytesSent;
                                }

                                #region
                                //NetworkStream stream = client.GetStream();
                                //if (stream.CanWrite)
                                //{
                                //stream.Write(data, 0, data.Length);
                                //int totalSent = 0;
                                //while (totalSent < data.Length)
                                //{
                                //    int bytesTransferred = stream.Socket.Send(data, totalSent, data.Length - totalSent, SocketFlags.None);
                                //    totalSent += bytesTransferred;
                                //}
                                //}
                                #endregion
                            }
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(50));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"Tcp Server listennig exception: {ex.GetBaseException().Message}{Environment.NewLine}{Environment.StackTrace}");
                        Restart();
                    }
                }
            });
        }

        /// <summary>
        /// QV notify Unreal client about blocks state has been changed (state has been changed by QV)
        /// </summary>
        protected override void SendMessageHandler(BlockStateUpdateArg updateArg)
        {
            var message = BlockStateSerialize(updateArg);
            logger.Debug($"Try to send message to External client: {message}");
            try
            {
                enqueuedMessages.Enqueue(message);
                logger.Debug($"Message has send to External client successfully");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"TcpServer sending message to External client exception: {ex.GetBaseException().Message}{Environment.NewLine}{Environment.StackTrace}");
            }
        }

        public void Start()
        {
            shouldStop = false;
            connections.Clear();
            tcpListener.Start();            
            logger.Debug("Tcp Server has been started");
        }

        public void Stop()
        {
            shouldStop = true;
            tcpListener.Stop();
            foreach (var connection in connections)
                connection.Close();
            connections.Clear();
            logger.Debug("Tcp Server has been stopped");
        }

        private void Restart()
        {
            logger.Debug("Tcp Server restarting");
            foreach (var connection in connections)
                connection.Close();
            connections.Clear();
            tcpListener.Stop();
            tcpListener.Start();
            logger.Debug("Tcp Server has been restarted");
        }
    }
}