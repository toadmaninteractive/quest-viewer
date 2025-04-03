using QuestGraph.Core.Protocol;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core.TelerikDrawingModel
{
    public class BlockAction : BlockBase
    {
        public override NodeType Type => NodeType.Action;

        public BlockAction(DomenModel.BlockAction domenBlockModel, ConnectorCollection connectors) : base(domenBlockModel, connectors)
        {

        }
    }
}