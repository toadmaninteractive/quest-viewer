using QuestGraph.Core.DomenModel;
using System.Collections.Generic;

namespace QuestGraph.Core
{
    public interface IDrawingFactory
    {
        DrawingConfiguration DrawingConfig { get; }
        object GetStateBlockDrawingModel(DomenModel.BlockState protocolSource);
        object GetActionBlockDrawingModel(DomenModel.BlockAction protocolSource);
        object GetGraphDrawingModel(GraphDomenModel graphDomenModel);
        object GetBlockGroupDrawingModel(BlockGroup protocolSource);
    }
}