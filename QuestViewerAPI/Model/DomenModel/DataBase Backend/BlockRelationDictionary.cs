using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuestGraph.Core
{
    public class BlockRelationDictionary : IEnumerable
    {
        public class BlockRelationSet
        {
            public List<string> OutputBlocks = new List<string>();
            public List<string> InputBlocks = new List<string>();

            public BlockRelationSet() 
            { }

            public void AppendOutput(string relationBlock)
            {
                OutputBlocks.Add(relationBlock);
            }

            public void AppendInput(string relationBlock)
            {
                InputBlocks.Add(relationBlock);
            }
        }

        private Dictionary<string, BlockRelationSet> blockRelations = new Dictionary<string, BlockRelationSet>();

        public int Count => blockRelations.Count;

        public BlockRelationSet GetBlockRelationSet(string key)
        {
            return blockRelations.ContainsKey(key) ? blockRelations[key] : new BlockRelationSet();
        }

        public KeyValuePair<string, BlockRelationSet> ElementAt(int index)
        {
            return blockRelations.ElementAt(index);
        }

        public bool ContainsKey(string key)
        {
            return blockRelations.ContainsKey(key);
        }

        public void AddRelation(string fromBlockRefId, string toBlockRefId)
        {
            if (!blockRelations.ContainsKey(fromBlockRefId))
                blockRelations[fromBlockRefId] = new BlockRelationSet();
            blockRelations[fromBlockRefId].AppendOutput(toBlockRefId);

            if (!blockRelations.ContainsKey(toBlockRefId))
                blockRelations[toBlockRefId] = new BlockRelationSet();
            blockRelations[toBlockRefId].AppendInput(fromBlockRefId);
        }

        public IEnumerator GetEnumerator()
        {
            return blockRelations.GetEnumerator();
        }
    }
}