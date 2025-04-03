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
    public class StatusCheck : ResponsiveRequest
    {
        public static string ExpectedTypeName => "status_check";

        public StatusCheck(ImmutableJsonObject source) : base(source)
        {
        }

        public override void Invoke(IInterProcessCommunication interProcessCommunicationModel)
        {
            HasResponsePrepared = true;
            base.Invoke(interProcessCommunicationModel);
        }

        public override Response GetResponse()
        {
            return new Response(source);
        }
    }
}