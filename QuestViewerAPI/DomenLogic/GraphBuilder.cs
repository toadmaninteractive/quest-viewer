using QuestGraph.Core.DomenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.Diagrams.Extensions.ViewModels;

namespace QuestGraph.Core
{
    public class GraphBuilder
    {
        public GraphBuilder() 
        { }

        public GraphDomenModel Build(List<DomenModel.Edge> edgesDomenModels, IEnumerable<DomenModel.IBlock> blockDomenModels, IEnumerable<DomenModel.BlockGroup> blockGroupDomenModels)
        {
            var edges = new List<DomenModel.Edge>();
            var blocks = new List<DomenModel.IBlock>();
            var blockGroups = new List<DomenModel.BlockGroup>();

            var invalidBlockIds = blockDomenModels.Where(x => !x.IsValid).Select(x => x.Id);
            var collapsedGroups = blockGroupDomenModels.Where(x => !x.IsExpanded).ToList();
            var edgesInsideGroups = new List<DomenModel.Edge>();
            foreach (var collapsedBlockIds in collapsedGroups.Select(x => x.Nodes))
            {
                var edgesInsideGroup = edgesDomenModels.Where(x => collapsedBlockIds.Contains(x.From) && collapsedBlockIds.Contains(x.To)).ToList();
                if (edgesInsideGroup.Any())
                    edgesInsideGroups.AddRange(edgesInsideGroup);
            }

            foreach (var collapsedGroup in collapsedGroups)
                blockGroups.Add(collapsedGroup);

            var groupingBlockIds = collapsedGroups.SelectMany(x => x.Nodes);
            foreach (var blockDomenModel in blockDomenModels.Where(x => !groupingBlockIds.Contains(x.Id)))
                blocks.Add(blockDomenModel);

            foreach (var edge in edgesDomenModels.Except(edgesInsideGroups).Where(x => !invalidBlockIds.Contains(x.From) && !invalidBlockIds.Contains(x.To)))
                edges.Add(edge);

            var blockIds = blocks.Select(x => x.Id);
            var blockIsGroupIds = blockGroups.SelectMany(x => x.Nodes);
            var intersectIds = blockIds.Intersect(blockIsGroupIds);
            if (intersectIds.Any())
                throw new InvalidOperationException($"{Texts.GraphBuilder.BlockIdIntersect} {string.Join(", ", intersectIds)}{Environment.NewLine}{Environment.StackTrace}");


            return new GraphDomenModel(blocks, edges, blockGroups);
        }
    }
}