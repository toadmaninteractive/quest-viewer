using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core.TelerikDrawingModel
{
    public abstract class BlockShapeBase : RadDiagramShape
    {
        public static readonly DependencyProperty CustomConnectorsProperty =
            DependencyProperty.Register("CustomConnectors",
            typeof(ConnectorCollection),
            typeof(BlockShapeBase),
            new FrameworkPropertyMetadata(OnCustomConnectorsBind));

        public ConnectorCollection CustomConnectors
        {
            get => (ConnectorCollection)GetValue(CustomConnectorsProperty);
            set => SetValue(CustomConnectorsProperty, value);
        }

        protected IGraphInteractive graphInteractive;

        protected BlockShapeBase(double width, double height, IGraphInteractive graphInteractive) 
        {
            this.graphInteractive = graphInteractive;
            MinWidth = Constants.Numerical.BlockMinWidth;
            MinHeight = Constants.Numerical.BlockMinHeight;
            Width = width;
            Height = height;
            Geometry = null;
        }

        protected BlockShapeBase(double width, double height, double minWidth, double minHeight, IGraphInteractive graphInteractive)
        {
            this.graphInteractive = graphInteractive;
            MinWidth = minWidth;
            MinHeight = minHeight;
            Width = width;
            Height = height;
            Geometry = null;
        }

        protected static void OnCustomConnectorsBind(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockShapeBase target)
            {
                target.SetCustomConnectors(e.OldValue as ConnectorCollection, e.NewValue as ConnectorCollection);
            }
        }

        protected void SetCustomConnectors(ConnectorCollection oldCollection, ConnectorCollection newCollection)
        {
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= Connectors_CollectionChanged;
            }
            Connectors.Clear();
            if (newCollection != null)
            {
                foreach (var item in newCollection!.Cast<IConnector>())
                    Connectors.Add(item);
                newCollection.CollectionChanged += Connectors_CollectionChanged;
            }
        }

        protected void Connectors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems!.Cast<IConnector>())
                        Connectors.Add(item);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems!.Cast<IConnector>())
                        Connectors.Remove(item);
                    break;
            }
        }
    }
}