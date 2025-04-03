using QuestGraph.Core.DomenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public interface IDrawingModelService
    {
        IEnumerable<IBlock> GetSelectedBlockDomenModels(object graphDrawingModel);
        IEnumerable<BlockGroup> GetSelectedBlockGroupDomenModels(object graphDrawingModel);
    }
}