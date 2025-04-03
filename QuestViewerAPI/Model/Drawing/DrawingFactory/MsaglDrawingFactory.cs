using Microsoft.Msagl.Drawing;
using QuestGraph.Core.DomenModel;
using System.Collections.Generic;

namespace QuestGraph.Core
{
    public class MsaglDrawingFactory : IDrawingFactory
    {
        public DrawingConfiguration DrawingConfig => new DrawingConfiguration(BlockInteractionMode.Viewer, BlockConnectorMode.TopBottom, EdgesRoutingMode.Non);

        public object GetStateBlockDrawingModel(DomenModel.BlockState domenSource)
        {
            var node = new Node(domenSource.Id);
            node.Attr.Shape = Shape.Box;
            node.Attr.LineWidth = 1;
            node.Attr.Color = Color.LightSlateGray;
            node.Attr.FillColor = domenSource.IsActive ? Color.LightSteelBlue : Color.LightGray;
            node.Attr.InitialCoordinate = new Microsoft.Msagl.Core.Geometry.Point(domenSource.Position.X, domenSource.Position.Y);
            node.Attr.CustomHeight = domenSource.Size.Height;
            node.Attr.CustomWidth = domenSource.Size.Width;
            node.LabelText = domenSource.Caption;
            return node;
        }

        public object GetActionBlockDrawingModel(DomenModel.BlockAction domenSource)
        {
            var node = new Node(domenSource.Id);
            node.Attr.Shape = Shape.Box;
            node.Attr.LineWidth = 1;
            node.Attr.Color = Color.Gold;
            node.Attr.FillColor = domenSource.IsActive ? Color.Yellow : Color.LightYellow;
            node.Attr.InitialCoordinate = new Microsoft.Msagl.Core.Geometry.Point(domenSource.Position.X, domenSource.Position.Y);
            node.Attr.CustomHeight = domenSource.Size.Height;
            node.Attr.CustomWidth = domenSource.Size.Width;
            node.LabelText = domenSource.Caption;

            return node;
        }

        public object GetGraphDrawingModel(GraphDomenModel graphDomenModel)
        {
            var graphDrawingModel = new Graph();
            foreach (var blockDomenModel in graphDomenModel.Blocks)
            {
                var blockDrawingModel = (Node)blockDomenModel.GetDrawingModel();
                graphDrawingModel.AddNode(blockDrawingModel);
            }

            foreach (var edgeProtocol in graphDomenModel.Edges)
            {
                var edge = graphDrawingModel.AddEdge(edgeProtocol.From, edgeProtocol.To);
                if (edgeProtocol.Type == Protocol.EdgeType.Bidirectional)
                    edge.Attr.ArrowheadAtSource = ArrowStyle.Normal;
            }

            return graphDrawingModel;
        }

        public object GetBlockGroupDrawingModel(BlockGroup protocolSource)
        {
            throw new System.NotImplementedException();
        }
    }
}
