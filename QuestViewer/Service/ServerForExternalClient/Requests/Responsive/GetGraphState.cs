using Json;
using Json.Serialization;
using QuestGraph.Core;
using QuestViewer.ExternalClientResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.ExternalClientRequests
{
    public class GraphStateDataJsonParser
    {
        public Json.ImmutableJson Serialize(GraphStateTcpData value)
        {
            var json = new Json.JsonObject();
            json["type"] = value.Type;
            json["db"] = value.DataBase;
            json["doc_id"] = value.DocumentId;
            json["nodes"] = JsonSerializer.Dict(JsonSerializer.String, JsonSerializer.Bool).Serialize(value.Nodes);
            return json;
        }

        public GraphStateTcpData Deserialize(Json.ImmutableJson json)
        {
            Dictionary<string, bool> activeBlocks = new Dictionary<string, bool>();
            if (json.AsObject.TryGetValue("nodes", out var jsonActiveBlocks) && !jsonActiveBlocks.IsNull)
                activeBlocks = JsonSerializer.Dict(JsonSerializer.String, JsonSerializer.Bool).Deserialize(jsonActiveBlocks);

            var doc_id = JsonSerializer.String.Deserialize(json["doc_id"]);
            var db = JsonSerializer.String.Deserialize(json["db"]);
            var type = JsonSerializer.String.Deserialize(json["type"]);

            return new GraphStateTcpData
            {
                Type = type,
                DataBase = db,
                DocumentId = doc_id,
                Nodes = activeBlocks
            };
        }

        public GraphStateTcpData Deserialize(string json)
        {
            return Deserialize(JsonParser.Parse(json));
        }
    }

    public class GraphStateTcpData
    {
        public string Type {  get; set; }
        public string DataBase { get; set; }
        public string DocumentId { get; set; }
        public Dictionary<string, bool> Nodes { get; set; }
    }

    /// <summary>
    /// External client requests information about all states blocks
    /// </summary>
    public class GraphState : ResponsiveRequest
    {
        public static string ExpectedTypeName => "get_current_state";
        private GraphStateTcpData actualBlockStates;

        public GraphState(ImmutableJsonObject source) : base(source)
        {
        }

        public override Response GetResponse()
        {
            var jsonObject = new GraphStateDataJsonParser().Serialize(actualBlockStates);
            return new Response(jsonObject);
        }

        public override void Invoke(IInterProcessCommunication interProcessCommunicationModel)
        {
            var activeBlocks = interProcessCommunicationModel.GetStates();
            actualBlockStates = new GraphStateTcpData 
            { 
                Type = this.Type, 
                Nodes = activeBlocks, 
                DocumentId = interProcessCommunicationModel.ActualDocumentId, 
                DataBase = interProcessCommunicationModel.ActualDatabase 
            };
            HasResponsePrepared = true;
        }
    }
}