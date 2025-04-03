using Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public class GameLevelMonitor
    {
        public ImmutableJson GraphStateOnLevelStart { get; set; }
    }
}