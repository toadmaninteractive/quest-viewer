using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuestGraph.Core.DomenModel
{
    public class BlockGroup
    {
        public string Name => protocolModel.Name;
        public Point Position => new Point(protocolModel.Layout.X, protocolModel.Layout.Y);
        public Point Center => new Point(protocolModel.Layout.X + protocolModel.Layout.Width / 2, protocolModel.Layout.Y + protocolModel.Layout.Height / 2);
        public Size Size => new Size(protocolModel.Layout.Width, protocolModel.Layout.Height);
        public List<string> Nodes => protocolModel.Nodes;
        public bool IsExpanded => protocolModel.IsExpanded;
        public Protocol.NodeGroup ProtocolModel => protocolModel;
        public int StateCount { get; }
        public int ActionCount { get; }

        private readonly Protocol.NodeGroup protocolModel;
        private readonly IDrawingFactory drawingFactory;

        public BlockGroup(Protocol.NodeGroup protocolModel, int stateCount, int actionCount, IDrawingFactory drawingFactory)
        {
            this.protocolModel = protocolModel;
            StateCount = stateCount;
            ActionCount = actionCount;
            this.drawingFactory = drawingFactory;
        }

        public object GetDrawingModel()
        {
            return drawingFactory.GetBlockGroupDrawingModel(this);
        }

        public void UpdateLayoutPosition(double x, double y)
        {
            protocolModel.Layout.X = x;
            protocolModel.Layout.Y = y;
        }

        public void UpdateLayoutSize(Size size)
        {
            protocolModel.Layout.Width = size.Width;
            protocolModel.Layout.Height = size.Height;
        }

        public void ExpandedSwitch()
        {
            protocolModel.IsExpanded = !protocolModel.IsExpanded;
        }
    }
}