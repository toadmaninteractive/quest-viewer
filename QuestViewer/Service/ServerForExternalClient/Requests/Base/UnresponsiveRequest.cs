using Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.ExternalClientRequests
{
    public abstract class UnresponsiveRequest : Request
    {
        protected UnresponsiveRequest(ImmutableJsonObject source) : base(source)
        {
        }
    }
}