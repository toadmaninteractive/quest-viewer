using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core.DomenModel
{
    public interface IBlockState
    {
        string Id { get; }
        bool IsActive { get; }
        void SetState(bool value);
    }
}