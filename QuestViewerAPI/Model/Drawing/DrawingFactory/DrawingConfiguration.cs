using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core
{
    public struct DrawingConfiguration
    {
        public BlockInteractionMode InteractionMode;
        public BlockConnectorMode ConnectorMode;
        public EdgesRoutingMode RoutingMode;

        public DrawingConfiguration(BlockInteractionMode interactionMode, BlockConnectorMode connectorMode, EdgesRoutingMode routingMode)
        {
            InteractionMode = interactionMode;
            ConnectorMode = connectorMode;
            RoutingMode = routingMode;
        }
    }
}