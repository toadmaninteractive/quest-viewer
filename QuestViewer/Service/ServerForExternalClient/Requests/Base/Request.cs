using Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestViewer.ExternalClientResponses;
using NLog;

namespace QuestViewer.ExternalClientRequests
{
    public abstract class Request
    {
        public string DataBaseName;
        public string DocId;
        public string Type;

        protected ImmutableJsonObject source;
        protected Logger logger = LogManager.GetCurrentClassLogger();

        public Request(ImmutableJsonObject source) 
        {
            this.source = source;
            DeserializeRequiredFields();
            DeserializeCustomFields();
        }

        public virtual void Invoke(QuestGraph.Core.IInterProcessCommunication interProcessCommunicationModel)
        { }

        protected void DeserializeRequiredFields()
        {
            Type = source["type"].AsString;
            DataBaseName = source["db"].AsString;
            DocId = source["doc_id"].AsString;
        }

        protected virtual void DeserializeCustomFields()
        { }
    }
}