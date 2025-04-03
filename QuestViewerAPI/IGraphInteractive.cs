using QuestGraph.Core.DomenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public interface IGraphInteractive
    {
        event Action OnStatesUpdate;
        event Action OnBlockGroupCollectionChanged;
        event Action<string, bool> OnBlockGroupExpandChanged;
        void InteractWithBlock(DomenModel.IBlock blockId);
        void UpdateDomenModelBlockPosition(DomenModel.IBlock block, double x, double y);
        void BlockSizeChanged(DomenModel.IBlock block, System.Windows.Size size);
        void BlockGroupSizeChanged(DomenModel.BlockGroup blockGroup, System.Windows.Size size);

        void InteractWithBlockGroup(BlockGroup blockGroupDomenModel);
    }
}