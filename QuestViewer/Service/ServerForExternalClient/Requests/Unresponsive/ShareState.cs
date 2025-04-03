using Json;
using QuestGraph.Core;
using QuestViewer.ExternalClientResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.ExternalClientRequests
{
    /// <summary>
    /// External client notify QV about blocks states have been changed (by External client)
    /// </summary>
    public class ShareState : UnresponsiveRequest
    {
        public static string ExpectedTypeName => "share_state";

        private Dictionary<string, bool> updatedNodes { get; set; }

        public ShareState(ImmutableJsonObject source) : base(source)
        {
        }

        protected override void DeserializeCustomFields()
        {
            var jsonObject = new GraphStateDataJsonParser().Deserialize(source);
            updatedNodes = jsonObject.Nodes;
        }

        public override void Invoke(IInterProcessCommunication interProcessCommunicationModel)
        {
            interProcessCommunicationModel.SetStateFromExternalClient(updatedNodes);
        }
    }
}