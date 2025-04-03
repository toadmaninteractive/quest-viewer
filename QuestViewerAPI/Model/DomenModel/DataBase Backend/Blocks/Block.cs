using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuestGraph.Core.DomenModel
{
    public abstract class Block
    {
        public string Id => protocolSource.RefId;
        public Point Position => new Point(protocolSource.Layout.X, protocolSource.Layout.Y);
        public Point Center => new Point(protocolSource.Layout.X + protocolSource.Layout.Width / 2, protocolSource.Layout.Y + protocolSource.Layout.Height / 2);
        public Size Size => new Size(protocolSource.Layout.Width, protocolSource.Layout.Height);
        public bool IsValid => !StructureValidationResults.OfType<GraphItemValidationError>().Any();
        public Protocol.NodeType Type => protocolSource.Type;
        public string Caption => protocolSource.Name;
        public List<GraphItemValidationResult> StructureValidationResults { get; private set; } = new List<GraphItemValidationResult>();
        public int IndexInDomenList {  get; private set; }

        protected readonly IDrawingFactory drawingFactory;
        protected Protocol.Node protocolSource;

        public Block(Protocol.Node protocolSource, IDrawingFactory drawingFactory, int index)
        {
            this.drawingFactory = drawingFactory;
            this.protocolSource = protocolSource;
            IndexInDomenList = index;
        }

        public void UpdateLayoutPosition(double x, double y)
        {
            protocolSource.Layout.X = x;
            protocolSource.Layout.Y = y;
        }

        public void UpdateLayoutSize(Size size)
        {
            protocolSource.Layout.Width = size.Width;
            protocolSource.Layout.Height = size.Height;
        }

        public void AppendValidationResult(GraphItemValidationResult error)
        {
            StructureValidationResults.Add(error);
        }
    }
}