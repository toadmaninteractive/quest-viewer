using QuestGraph.Core.Protocol;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core.TelerikDrawingModel
{
    public class BlockState : BlockBase
    {
        public override NodeType Type => NodeType.State;

        public BlockState(DomenModel.BlockState domenBlockModel, ConnectorCollection connectors) : base(domenBlockModel, connectors) 
        {
            
        }
    }
}