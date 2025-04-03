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
    public class LevelStart : ResponsiveRequest
    {
        public static string ExpectedTypeName => "level_start";
        private GraphStateTcpData response = null;
        private GraphStateDataJsonParser dataJsonParser = new GraphStateDataJsonParser();

        public LevelStart(ImmutableJsonObject source) : base(source)
        {
        }

        public override void Invoke(QuestGraph.Core.IInterProcessCommunication interProcessCommunicationModel)
        {
            var actualStates = interProcessCommunicationModel.GetStates();
            var graphStateObject = new GraphStateTcpData()
            {
                DataBase = this.DataBaseName,
                DocumentId = this.DocId,
                Type = this.Type,
                Nodes = actualStates
            };
            interProcessCommunicationModel.ExternalClientLevelMonitor.GraphStateOnLevelStart = dataJsonParser.Serialize(graphStateObject);
            response = new GraphStateTcpData()
            {
                DataBase = this.DataBaseName,
                DocumentId = this.DocId,
                Type = ShareState.ExpectedTypeName, //Type has to be "share_state"
                Nodes = actualStates
            };
            HasResponsePrepared = true;
        }

        public override Response GetResponse()
        {
            var responseObj = dataJsonParser.Serialize(response);
            return new Response(responseObj);
        }
    }
}