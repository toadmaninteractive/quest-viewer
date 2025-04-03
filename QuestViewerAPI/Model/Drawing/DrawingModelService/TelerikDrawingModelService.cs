using QuestGraph.Core.DomenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public class TelerikDrawingModelService : IDrawingModelService
    {
        public IEnumerable<IBlock> GetSelectedBlockDomenModels(object graphDrawingModel)
        {
            var telerikDrawingModel = graphDrawingModel as TelerikDrawingModel.Graph;
            return telerikDrawingModel.Items.OfType<TelerikDrawingModel.BlockBase>().Where(x => x.IsSelected).Select(x => x.DomenBlockModel).ToList();
        }

        public IEnumerable<BlockGroup> GetSelectedBlockGroupDomenModels(object graphDrawingModel)
        {
            var telerikDrawingModel = graphDrawingModel as TelerikDrawingModel.Graph;
            var result = telerikDrawingModel.Items.OfType<TelerikDrawingModel.BlockGroup>().Where(x => x.IsSelected).Select(x => x.BlockGroupDomenModel).ToList();
            result.AddRange(telerikDrawingModel.SelectedGroupDomenModel);
            return result;
        }
    }
}