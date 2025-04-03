using Json;
using QuestViewer.ExternalClientResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.ExternalClientRequests
{
    public abstract class ResponsiveRequest : Request
    {
        public bool HasResponsePrepared { get; protected set; }

        public ResponsiveRequest(ImmutableJsonObject source) : base(source)
        {
        }

        public abstract Response GetResponse();
    }
}