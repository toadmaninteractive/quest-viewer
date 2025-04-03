using Json;
using QuestViewer.ExternalClientResponses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.ExternalClientRequests
{
    public class LevelFinish : ResponsiveRequest
    {
        public static string ExpectedTypeName => "level_finish";
        private GraphStateTcpData response = null;
        private GraphStateDataJsonParser dataJsonParser = new GraphStateDataJsonParser();

        public LevelFinish(ImmutableJsonObject source) : base(source)
        {
        }

        public override void Invoke(QuestGraph.Core.IInterProcessCommunication interProcessCommunicationModel)
        {
            if (interProcessCommunicationModel.ExternalClientLevelMonitor.GraphStateOnLevelStart == null)
                return;

            var blockStatesFromLevelStart = dataJsonParser.Deserialize(interProcessCommunicationModel.ExternalClientLevelMonitor.GraphStateOnLevelStart);
            response = new GraphStateTcpData()
            {
                DataBase = this.DataBaseName,
                DocumentId = this.DocId,
                Type = ShareState.ExpectedTypeName, //Type has to be "share_state"
                Nodes = blockStatesFromLevelStart.Nodes
            };
            HasResponsePrepared = true;
            interProcessCommunicationModel.ApplyActiveBlocksWithoutResponseToExternalClient(response.Nodes);
        }

        public override Response GetResponse()
        {
            var responseObj = dataJsonParser.Serialize(response);
            return new Response(responseObj);
        }
    }
}