using Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer.ExternalClientResponses
{
    public class Response
    {
        private ImmutableJson jsonObject;

        public Response(ImmutableJson jsonObject)
        {
            this.jsonObject = jsonObject;
        }

        public string Serialization()
        {
            return jsonObject.ToString();
        }
    }
}