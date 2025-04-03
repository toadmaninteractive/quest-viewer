using Json;
using System.Collections.Generic;
using System.Linq;

namespace QuestGraph.Core.DomenModel
{
    public class BlockAction : Block, IBlock
    {
        public bool IsActive => inputBlocks.Count == 0 || inputBlocks.All(x => x.IsActive);
        
        private List<IBlockState> inputBlocks = new List<IBlockState>();
        private List<IBlockState> outputBlocks = new List<IBlockState>();

        public BlockAction(Protocol.NodeAction protocolSource, IDrawingFactory drawingFactory, int index) : base(protocolSource, drawingFactory, index)
        {
        }

        public void SetRelations(List<IBlockState> inputBlocks, List<IBlockState> outputBlocks)
        {
            this.inputBlocks = inputBlocks;
            this.outputBlocks = outputBlocks;
        }

        public void Interact()
        {
            if (!IsActive)
                return;

            foreach (IBlockState block in inputBlocks) block.SetState(false);
            foreach (IBlockState block in outputBlocks) block.SetState(true);
        }

        public object GetDrawingModel()
        {
            return drawingFactory.GetActionBlockDrawingModel(this);
        }

        public void UpdateProtocol(ImmutableJson json)
        {
            var blockStateJsonSerializer = new NodeActionJsonSerializer();
            var actualProtocol = blockStateJsonSerializer.Deserialize(json);

            UpdateLayoutPosition(actualProtocol.Layout.X, actualProtocol.Layout.Y);
            UpdateLayoutSize(new System.Windows.Size(actualProtocol.Layout.Width, actualProtocol.Layout.Height));
            var protocolSourceAction = (Protocol.NodeAction)protocolSource;
            protocolSourceAction.Name = actualProtocol.Name;
            protocolSourceAction.Description = actualProtocol.Description;
            protocolSourceAction.RefId = actualProtocol.RefId;
        }
    }
}