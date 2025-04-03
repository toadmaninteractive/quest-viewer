using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.Diagrams.Extensions.ViewModels;
using Telerik.Windows.Data;

namespace QuestGraph.Core.TelerikDrawingModel
{
    public class Graph : ObservableGraphSourceBase<NodeViewModelBase, LinkViewModelBase<NodeViewModelBase>>
    {
        private class GroupSetItem
        {
            public Telerik.Windows.Diagrams.Core.IGroup DrawObject { get; }
            public DomenModel.BlockGroup DomenModel { get; }

            public GroupSetItem(Telerik.Windows.Diagrams.Core.IGroup drawObject, DomenModel.BlockGroup domenModel)
            {
                DrawObject = drawObject;
                DomenModel = domenModel;
            }
        }

        public IEnumerable<DomenModel.BlockGroup> SelectedGroupDomenModel => blockGroupSet.Values.Where(x => x.DrawObject.IsSelected).Select(x => x.DomenModel);

        private Dictionary<string, NodeViewModelBase> drawingModelOfValidBlocks = new Dictionary<string, NodeViewModelBase>();
        private Dictionary<string, GroupSetItem> blockGroupSet = new Dictionary<string, GroupSetItem>();

        public Graph()
        {
        }

        public void AddBlockGroup(BlockGroup blockGroupDrawingModel)
        {
            base.AddNode(blockGroupDrawingModel);
            foreach (var blockId in blockGroupDrawingModel.Blocks)
                drawingModelOfValidBlocks.Add(blockId, blockGroupDrawingModel);
        }

        public void AddBlock(BlockBase blockDrawingModel)
        {
            base.AddNode(blockDrawingModel);
            if (blockDrawingModel.IsValid)
                drawingModelOfValidBlocks.Add(blockDrawingModel.Id, blockDrawingModel);
        }

        public void AddEdge(Connection edge)
        {
            //Relations has to set only for valid blocks and edges (in QuestGraph.Core.Quest.BuildDomenModels() method)
            edge.Source = drawingModelOfValidBlocks[edge.From];
            edge.Target = drawingModelOfValidBlocks[edge.To];
            AddLink(edge);
        }

        public void BindExpandedGroupBorderObjectToDomenModel(Telerik.Windows.Diagrams.Core.IGroup group, DomenModel.BlockGroup domenModel)
        {
            blockGroupSet.Add(domenModel.Name, new GroupSetItem(group, domenModel));
        }
    }
}