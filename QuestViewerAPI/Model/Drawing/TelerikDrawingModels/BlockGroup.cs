using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Controls.Diagrams.Extensions.ViewModels;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core.TelerikDrawingModel
{
    /// <summary>
    /// Drawing object for collapsing state
    /// </summary>
    public class BlockGroup : NodeViewModelBase
    {
        public ConnectorCollection Connectors { get; }
        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }
        public List<string> Blocks => domenModel.Nodes;
        public int StateCount => domenModel.StateCount;
        public int ActionCount => domenModel.ActionCount;
        public DomenModel.BlockGroup BlockGroupDomenModel => domenModel;

        private string caption;
        private readonly DomenModel.BlockGroup domenModel;

        public BlockGroup(DomenModel.BlockGroup domenModel, ConnectorCollection connectorCollection)
        {
            this.domenModel = domenModel;
            Position = new Point(domenModel.Position.X, domenModel.Position.Y);
            Content = new TextBlock();
            Width = domenModel.Size.Width;
            Height = domenModel.Size.Height;
            Caption = domenModel.Name;
            Connectors = connectorCollection;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(Position))
                domenModel.UpdateLayoutPosition(Position.X, Position.Y);
            if (propertyName == nameof(Width))
                domenModel.UpdateLayoutSize(new Size(Width, domenModel.Size.Height));
            if (propertyName == nameof(Height))
                domenModel.UpdateLayoutSize(new Size(domenModel.Size.Width, Height));

            base.OnPropertyChanged(propertyName);
        }
    }
}