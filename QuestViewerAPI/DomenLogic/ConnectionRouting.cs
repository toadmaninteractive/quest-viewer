using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core
{
    public static class ConnectionRouting
    {
        public static AStarRouter DefaultRouter(RadDiagram radDiagram)
        {
            return new AStarRouter(radDiagram)
            {
                AvoidConnectionOverlap = false,
                AvoidShapes = true,
                SegmentOverlapDistance = 10,
                SegmentOverlapPenalty = 10,
                ShapeCrossPenalty = 10,
                WallOptimization = true
            };
        }
    }
}