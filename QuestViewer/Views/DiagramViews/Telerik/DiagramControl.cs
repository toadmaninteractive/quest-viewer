using System.Linq;
using System.Windows;
using QuestGraph.Core.TelerikDrawingModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using QuestGraph.Core;

namespace QuestViewer.TelerikDrawing
{
    public class DiagramControl : RadDiagram
    {
        private QuestViewerViewModel context;
        private Point previousPosition;
        private IRouter connectionRouter;

        public DiagramControl()
        {
            DataContextChanged += DataContextChangedHandler;
            MouseMove += MouseMoveHandler;
            PreviewMouseDown += PreviewMouseDownHandler;
            PreviewMouseUp += PreviewMouseUpHandler;
            AllowCopy = false;
            AllowCut = false;
            AllowPaste = false;
            AllowDelete = false;
            ConnectionBridge = BridgeType.Bow;

            connectionRouter = ConnectionRouting.DefaultRouter(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            var selectedBlockGroup = Groups.FirstOrDefault(x => x.IsSelected);
            if (selectedBlockGroup == null)
            {
                context.SelectedBlockGroup = null;
                return;
            }

            context.SelectedBlockGroup = context.BlockGroupInfos.FirstOrDefault(x => x.Name == selectedBlockGroup.Name);
        }


        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (context != null)
            {
                context.OnGraphRedraw -= GraphRebuildHandler;
                context.OnConnectionRouting -= ConnectionRoutingHandler;
                context.OnGraphClean -= GraphCleanHandler;
                context.OnBlockGroupSelected -= BlockGroupSelectedHandler;
                context.InteractiveModel.OnStatesUpdate -= StatesUpdateHandler;
                context.InteractiveModel.OnBlockGroupExpandChanged -= BlockGroupExpandChangedHandler;
                context.InteractiveModel.OnBlockGroupCollectionChanged -= GraphRedrawHandler;
            }
            context = (QuestViewerViewModel)DataContext;
            context.OnGraphRedraw += GraphRebuildHandler;
            context.OnConnectionRouting += ConnectionRoutingHandler;
            context.OnGraphClean += GraphCleanHandler;
            context.OnBlockGroupSelected += BlockGroupSelectedHandler;
            context.InteractiveModel.OnStatesUpdate += StatesUpdateHandler;
            context.InteractiveModel.OnBlockGroupExpandChanged += BlockGroupExpandChangedHandler;
            context.InteractiveModel.OnBlockGroupCollectionChanged += GraphRedrawHandler;

            UpdateRouteConnections();
        }

        private void GraphRebuildHandler(bool isAutoFit)
        {
            GraphRedrawHandler();
            if (isAutoFit) AutoFit();
        }

        private void GraphRedrawHandler()
        {
            GraphSource = (Graph)context.NextGraphDrawingModel;
            StatesUpdateHandler();
            ApplyExpandedBlockGroup();

            Application.Current.Dispatcher.BeginInvoke(() => context.IsBusy = false);
        }

        private void BlockGroupSelectedHandler(string blockGroupName)
        {
            foreach (var group in Groups)
                group.IsSelected = group.Name == blockGroupName;
        }

        private void StatesUpdateHandler()
        {
            Application.Current.Dispatcher.BeginInvoke(() => //Dispatcher becouse it has call from FileSystemWatcher thread
            {
                foreach (var dravingModel in Items.OfType<BlockBase>())
                    dravingModel.Refresh();
            });
        }

        private void BlockGroupExpandChangedHandler(string name, bool isExpanded)
        {
            GraphRedrawHandler();
            #region Reselect expahded\collapsed item
            var updatedGroupExpandedBorder = Groups.FirstOrDefault(x => x.Name == name);
            if (updatedGroupExpandedBorder != null)
                updatedGroupExpandedBorder.IsSelected = true;
            else
            {
                var updatedGroupBlock = Items.OfType<BlockGroup>().FirstOrDefault(x => x.BlockGroupDomenModel.Name == name);
                if (updatedGroupBlock != null)
                    updatedGroupBlock.IsSelected = true;
            }
            #endregion
        }

        private void ApplyExpandedBlockGroup()
        {
            var graphDrawingModel = (Graph)GraphSource;
            foreach (var blockGroupDomenModel in context.BlockGroupInfos.Where(x => x.IsExpanded))
            {
                IShape[] oddShapes = Shapes.Where(x => x.Content is BlockBase).Where(x => blockGroupDomenModel.Nodes.Contains(((BlockBase)x.Content).DomenBlockModel.Id)).ToArray();
                var group = Group(blockGroupDomenModel.Name, oddShapes);
                graphDrawingModel.BindExpandedGroupBorderObjectToDomenModel(group, blockGroupDomenModel);
            }
        }

        protected override IShape GetShapeContainerForItemOverride(object item)
        {
            if (item is BlockBase drawingBlock)
                return new BlockShape(drawingBlock.Width, drawingBlock.Height, context.InteractiveModel);
            else if (item is BlockGroup drawingBlockGroup)
                return new BlockGroupShape(drawingBlockGroup.Width, drawingBlockGroup.Height, context.InteractiveModel);
            else
                return base.GetShapeContainerForItemOverride(item);
        }

        void PreviewMouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton != MouseButtonState.Pressed)
                Mouse.OverrideCursor = Cursors.Arrow;
        }

        void PreviewMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                previousPosition = e.GetPosition(this);
                Mouse.OverrideCursor = Cursors.SizeAll;
            }
        }

        void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(this);
                Position += currentPosition - previousPosition;
                previousPosition = currentPosition;
            }
        }

        private void ConnectionRoutingHandler()
        {
            if (!context.IsLoaded)
                return;
            UpdateRouteConnections();
            GraphRedrawHandler();
        }

        private void UpdateRouteConnections()
        {
            RouteConnections = context.RoutingMode != EdgesRoutingMode.Non;
            ConnectionRoundedCorners = RouteConnections;
            if (RouteConnections)
                RoutingService.Router = connectionRouter;
        }

        private void GraphCleanHandler()
        {
            GraphSource = null;
        }

        public void Fitting()
        {
            AutoFit();
        }
    }
}