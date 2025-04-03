using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Msagl.Drawing;
using System.Windows.Input;
using QuestGraph.Core;

namespace QuestViewer
{
    public class MsaglPanelBehavior : Behavior<DockPanel>
    {
        GraphViewer graphViewer = new GraphViewer();
        QuestViewerViewModel viewModel => AssociatedObject.DataContext as QuestViewerViewModel;

        double currentXOffSet;
        double currentYOffSet;

        protected override void OnAttached()
        {
            graphViewer.MouseDown += GraphViewerMouseDownHandler;
            graphViewer.MouseUp += GraphViewerMouseUpHandler;
            graphViewer.BindToPanel(AssociatedObject);
            AssociatedObject.Loaded += LoadedHandler;
        }

        protected override void OnDetaching()
        {
            graphViewer.MouseDown -= GraphViewerMouseDownHandler;
            graphViewer.MouseUp -= GraphViewerMouseUpHandler;
            AssociatedObject.Loaded -= LoadedHandler;
        }

        private void LoadedHandler(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.OnGraphRedraw += GraphRebuildedHandler;
            viewModel.InteractiveModel.OnStatesUpdate += GraphRebuildedHandler;
        }

        private void GraphRebuildedHandler()
        {
            GraphRebuildedHandler(true);
        }

        private void GraphRebuildedHandler(bool isAutoFit)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() => //Dispatcher becouse it has call from FileSystemWatcher thread
            {
                graphViewer.NeedToCalculateLayout = false;
                graphViewer.Graph = (Graph)viewModel.NextGraphDrawingModel;
            });
        }

        private void GraphViewerMouseDownHandler(object sender, MsaglMouseEventArgs e)
        {
            currentXOffSet = e.X;
            currentYOffSet = e.Y;
        }

        private void GraphViewerMouseUpHandler(object sender, MsaglMouseEventArgs e)
        {
            var node = graphViewer.ObjectUnderMouseCursor as IViewerNode;
            throw new NotImplementedException();
            //if (node != null)
            //{
            //    if (currentXOffSet == e.X && currentYOffSet == e.Y)
            //        viewModel.InteractWithBlock(node.Node.);
            //    else
            //        viewModel.UpdateBlockPosition(node.Node.Id, node.Node.Pos.X, node.Node.Pos.Y);
            //}
        }
    }
}