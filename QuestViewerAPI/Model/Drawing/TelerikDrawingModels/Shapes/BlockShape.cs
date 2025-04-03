using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core.TelerikDrawingModel
{
    public class BlockShape : BlockShapeBase
    {
        public BlockShape(double width, double height, IGraphInteractive graphInteractive) : base(width, height, graphInteractive)
        {
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            graphInteractive.InteractWithBlock(((BlockBase)DataContext).DomenBlockModel);
        }

        protected override void OnPositionChanged(Point oldPosition, Point newPosition)
        {
            base.OnPositionChanged(oldPosition, newPosition);
            if (DataContext != null)
                graphInteractive.UpdateDomenModelBlockPosition(((BlockBase)DataContext).DomenBlockModel, newPosition.X, newPosition.Y);
        }

        protected override void OnSizeChanged(Size newSize, Size oldSize)
        {
            base.OnSizeChanged(newSize, oldSize);
            if (DataContext != null)
                graphInteractive.BlockSizeChanged(((BlockBase)DataContext).DomenBlockModel, newSize);
        }
    }
}