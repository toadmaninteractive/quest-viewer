using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace QuestGraph.Core.TelerikDrawingModel
{
    public class BlockGroupShape : BlockShapeBase
    {
        public BlockGroupShape(double width, double height, IGraphInteractive graphInteractive) : base(width, height, Constants.Numerical.BlockGroupMinWidth, Constants.Numerical.BlockGroupMinHeight, graphInteractive)
        {
        }

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            graphInteractive.InteractWithBlockGroup(((BlockGroup)DataContext).BlockGroupDomenModel);
            base.OnPreviewMouseDoubleClick(e);
        }

        protected override void OnSizeChanged(Size newSize, Size oldSize)
        {
            base.OnSizeChanged(newSize, oldSize);
            if (DataContext != null)
                graphInteractive.BlockGroupSizeChanged(((BlockGroup)DataContext).BlockGroupDomenModel, newSize);
        }
    }
}