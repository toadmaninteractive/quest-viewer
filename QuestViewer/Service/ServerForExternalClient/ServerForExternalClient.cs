using NLog;
using QuestGraph.Core;
using QuestViewer.ExternalClientRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.ServerForExternalClient
{
    public abstract class ServerForExternalClient
    {
        protected IInterProcessCommunication interProcessCommunicationModel;
        protected ExternalClientRequestRegister requestRegister = new ExternalClientRequestRegister();

        protected Logger logger = LogManager.GetCurrentClassLogger();
        protected bool shouldStop = false;

        public ServerForExternalClient(IInterProcessCommunication interProcessCommunicationModel)
        {
            this.interProcessCommunicationModel = interProcessCommunicationModel;
            this.interProcessCommunicationModel.OnStatesUpdateIPC += SendMessageHandler;
        }

        protected abstract void SendMessageHandler(BlockStateUpdateArg updateArg);

        protected string BlockStateSerialize(BlockStateUpdateArg updateArg)
        {
            var synchronizationData = new GraphStateTcpData
            {
                Type = requestRegister.RequestNameMapper[ExternalClientRequestRegister.RequestTypeNames.ShareState],
                Nodes = updateArg.Diff,
                DocumentId = updateArg.DocumentId,
                DataBase = updateArg.Database
            };
            var jsonObject = new GraphStateDataJsonParser().Serialize(synchronizationData);
            return jsonObject.ToString();
        }

        protected bool HandleRequest(string requestSource, out string responceText, string requestDividerSymbol = "")
        {
            responceText = string.Empty;
            var requestObj = requestRegister.GetRequestObject(requestSource);
            if (!requestObj.IsSuccess)
                return false;

            var request = requestObj.RequestObject;
            if (interProcessCommunicationModel.ActualDatabase != request.DataBaseName || interProcessCommunicationModel.ActualDocumentId != request.DocId)
                return false;

            request.Invoke(interProcessCommunicationModel);
            if (request is ExternalClientRequests.ResponsiveRequest responsiveRequest && responsiveRequest.HasResponsePrepared)
            {
                logger.Debug($"Server for external client try to serialize response text of {request.Type} request...");
                var response = responsiveRequest.GetResponse();
                responceText = response.Serialization() + requestDividerSymbol;
                logger.Debug($"Server for external client try to enqueue response text: {responceText}");
                return true;
            }
            else
            {
                logger.Debug("The Request is not responsive");
                return false;
            }
        }
    }
}