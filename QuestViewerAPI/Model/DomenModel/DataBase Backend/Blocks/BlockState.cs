using Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace QuestGraph.Core.DomenModel
{
    public class BlockState : Block, IBlock, IBlockState
    {
        public bool IsActive => ((Protocol.NodeState)protocolSource).IsActive;

        public BlockState(Protocol.NodeState protocolSource, IDrawingFactory drawingFactory, int index) : base(protocolSource, drawingFactory, index)
        {
            this.protocolSource = protocolSource;
        }

        public void SetState(bool value)
        {
            ((Protocol.NodeState)protocolSource).IsActive = value;
        }

        public void Interact()
        {
            SetState(!IsActive);
        }

        public object GetDrawingModel()
        {
            return drawingFactory.GetStateBlockDrawingModel(this);
        }

        public void UpdateProtocol(ImmutableJson json)
        {
            var blockStateJsonSerializer = new NodeStateJsonSerializer();
            var actualProtocol = blockStateJsonSerializer.Deserialize(json);

            UpdateLayoutPosition(actualProtocol.Layout.X, actualProtocol.Layout.Y);
            UpdateLayoutSize(new Size(actualProtocol.Layout.Width, actualProtocol.Layout.Height));
            var protocolSourceState = (Protocol.NodeState)protocolSource;
            protocolSourceState.IsActive = actualProtocol.IsActive;
            protocolSourceState.Name = actualProtocol.Name;
            protocolSourceState.Description = actualProtocol.Description;
            protocolSourceState.RefId = actualProtocol.RefId;
        }
    }
}