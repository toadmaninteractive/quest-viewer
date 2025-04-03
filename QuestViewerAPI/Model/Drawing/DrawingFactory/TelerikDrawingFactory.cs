using QuestGraph.Core.DomenModel;
using QuestGraph.Core.Protocol;
using QuestGraph.Core.TelerikDrawingModel;
using System;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core
{
    public class TelerikDrawingFactory : IDrawingFactory
    {
        public DrawingConfiguration DrawingConfig => drawingConfig;

        private readonly DrawingConfiguration drawingConfig;

        public TelerikDrawingFactory(DrawingConfiguration drawingMode)
        {
            this.drawingConfig = drawingMode;
        }

        public object GetStateBlockDrawingModel(DomenModel.BlockState domenSource)
        {
            return new TelerikDrawingModel.BlockState(domenSource, GetBlockConnectorCollection(domenSource.Type, domenSource.Id));
        }

        public object GetActionBlockDrawingModel(DomenModel.BlockAction domenSource)
        {
            return new TelerikDrawingModel.BlockAction(domenSource, GetBlockConnectorCollection(domenSource.Type, domenSource.Id));
        }

        public object GetBlockGroupDrawingModel(DomenModel.BlockGroup protocolSource)
        {
            var connectors = GetGroupConnectorCollection();
            return new TelerikDrawingModel.BlockGroup(protocolSource, connectors);
        }

        private ConnectorCollection GetBlockConnectorCollection(NodeType blockType, string blockId)
        {
            switch (drawingConfig.ConnectorMode)
            {
                case BlockConnectorMode.TopBottom:
                    return new TelerikTopBottomConnectorFactory().GetBlockConnectorCollection(drawingConfig.InteractionMode, blockType, blockId);
                case BlockConnectorMode.StandartOfFour:
                    return new TelerikStandartOfFourConnectorFactory().GetBlockConnectorCollection(drawingConfig.InteractionMode, blockType, blockId);
                default:
                    throw ExceptionConstructor.UnexpectedEnumValue(drawingConfig.ConnectorMode, Environment.StackTrace);
            }
        }

        private ConnectorCollection GetGroupConnectorCollection()
        {
            switch (drawingConfig.ConnectorMode)
            {
                case BlockConnectorMode.TopBottom:
                    return new TelerikTopBottomConnectorFactory().GetGroupConnectorCollection();
                case BlockConnectorMode.StandartOfFour:
                    return new TelerikStandartOfFourConnectorFactory().GetGroupConnectorCollection();
                default:
                    throw ExceptionConstructor.UnexpectedEnumValue(drawingConfig.ConnectorMode, Environment.StackTrace);
            }
        }

        public object GetGraphDrawingModel(GraphDomenModel graphDomenModel)
        {
            var drawingModel = new TelerikDrawingModel.Graph();

            foreach (var blockGroupDomenModel in graphDomenModel.BlockGroups)
            {
                var blockGroupDrawingModel = (TelerikDrawingModel.BlockGroup)blockGroupDomenModel.GetDrawingModel();
                drawingModel.AddBlockGroup(blockGroupDrawingModel);
            }

            foreach (var blockDomenModel in graphDomenModel.Blocks)
            {
                var blockDrawingModel = (TelerikDrawingModel.BlockBase)blockDomenModel.GetDrawingModel();
                blockDrawingModel.Refresh();
                drawingModel.AddBlock(blockDrawingModel);
            }

            foreach (var edgeDomenModel in graphDomenModel.Edges)
            {
                var edgeDrawingModel = new TelerikDrawingModel.Connection(edgeDomenModel);
                switch (drawingConfig.RoutingMode)
                {
                    case EdgesRoutingMode.Non:
                        edgeDrawingModel.UpdateConnectorPosition("Auto", "Auto");
                        break;
                    case EdgesRoutingMode.AStarRouting:
                        edgeDrawingModel.UpdateConnectorPosition(Connector.LinkDirection.Bottom.ToString(), Connector.LinkDirection.Top.ToString());
                        break;
                    default:
                        throw ExceptionConstructor.UnexpectedEnumValue(drawingConfig.RoutingMode, Environment.StackTrace);
                }
                drawingModel.AddEdge(edgeDrawingModel);
            }

            return drawingModel;
        }
    }
}