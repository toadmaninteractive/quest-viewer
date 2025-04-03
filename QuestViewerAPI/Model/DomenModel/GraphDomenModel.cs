using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core.DomenModel
{
    public class GraphDomenModel
    {
        public List<DomenModel.Edge> Edges {  get; set; }
        public IEnumerable<DomenModel.IBlock> Blocks { get; set; }
        public IEnumerable<DomenModel.BlockGroup> BlockGroups { get; set; }

        public GraphDomenModel(IEnumerable<IBlock> blocks, List<Edge> edges, IEnumerable<BlockGroup> blockGroups)
        {
            Blocks = blocks;
            Edges = edges;
            BlockGroups = blockGroups;
        }
    }
}