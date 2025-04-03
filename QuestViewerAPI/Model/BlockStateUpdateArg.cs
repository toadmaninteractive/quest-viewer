using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public class BlockStateUpdateArg
    {
        public Dictionary<string, bool> Diff;
        public string DocumentId;
        public string Database;

        public BlockStateUpdateArg(Dictionary<string, bool> diff, string documentId, string database)
        {
            Diff = diff;
            DocumentId = documentId;
            Database = database;
        }
    }
}