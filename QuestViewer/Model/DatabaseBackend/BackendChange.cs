using Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public class BackendChange
    {
        public string Seq { get; private set; }
        public string Id { get; private set; }
        public bool Deleted { get; private set; }
        public ImmutableJsonObject Doc { get; private set; }
        public string Rev { get; private set; }

        public static BackendChange Delete(string seq, string id)
        {
            return new BackendChange { Seq = seq, Id = id, Deleted = true };
        }

        public static BackendChange Update(string seq, string id, ImmutableJsonObject doc, string rev)
        {
            return new BackendChange { Seq = seq, Id = id, Doc = doc, Rev = rev };
        }

        public override string ToString()
        {
            return $"{Id} {Rev} {Doc != null}";
        }
    }
}