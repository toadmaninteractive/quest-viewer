using QuestGraph.Core.Protocol;
using QuestGraph.Core.TelerikDrawingModel;
using System.Windows;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core
{
    public class TelerikStandartOfFourConnectorFactory : TelerikConnectorFactory
    {
        public ConnectorCollection GetBlockConnectorCollection(BlockInteractionMode interactionMode, NodeType blockType, string blockId)
        {
            return new ConnectorCollection()
            {
                new Connector(Connector.LinkDirection.Top, blockType, blockId)
                {
                    Offset = new Point(0.5, 0),
                    Visibility = GetConnectorVisibility(interactionMode)
                },
                new Connector(Connector.LinkDirection.Bottom, blockType, blockId)
                {
                    Offset = new Point(0.5, 1),
                    Visibility = GetConnectorVisibility(interactionMode)
                },
                new Connector(Connector.LinkDirection.Left, blockType, blockId)
                {
                    Offset = new Point(0, 0.5),
                    Visibility = Visibility.Collapsed
                },
                new Connector(Connector.LinkDirection.Right, blockType, blockId)
                {
                    Offset = new Point(1, 0.5),
                    Visibility = Visibility.Collapsed
                }
            };
        }

        public ConnectorCollection GetGroupConnectorCollection()
        {
            return new ConnectorCollection()
            {
                new RadDiagramConnector()
                {
                    Offset = new Point(0.5, 0),
                    Name = Connector.LinkDirection.Top.ToString(),
                    Visibility = Visibility.Collapsed
                },
                new RadDiagramConnector()
                {
                    Offset = new Point(0.5, 1),
                    Name = Connector.LinkDirection.Bottom.ToString(),
                    Visibility = Visibility.Collapsed
                },
                new RadDiagramConnector()
                {
                    Offset = new Point(0, 0.5),
                    Name = Connector.LinkDirection.Left.ToString(),
                    Visibility = Visibility.Collapsed
                },
                new RadDiagramConnector()
                {
                    Offset = new Point(1, 0.5),
                    Name = Connector.LinkDirection.Right.ToString(),
                    Visibility = Visibility.Collapsed
                }
            };
        }
    } 
}