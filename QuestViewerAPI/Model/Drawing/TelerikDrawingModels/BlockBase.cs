using Json;
using QuestGraph.Core.DomenModel;
using QuestGraph.Core.Protocol;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Controls.Diagrams.Extensions.ViewModels;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core.TelerikDrawingModel
{
    public abstract class BlockBase : NodeViewModelBase
    {
        public IBlock DomenBlockModel => domenBlockModel;
        public string Id => domenBlockModel.Id;
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
        public bool IsActive
        {
            get { return isActive; }
            set 
            {
                isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
        public bool IsValid
        {
            get { return isVlalid; }
            set 
            { 
                isVlalid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }
        public abstract NodeType Type { get; }
        public string ToolTip
        {
            get { return toolTip; }
            set
            {
                toolTip = value;
                OnPropertyChanged(nameof(ToolTip));
            }
        }

        private bool isActive;
        private bool isVlalid;
        private string caption;
        private string toolTip;
        private IBlock domenBlockModel;

        public BlockBase(IBlock domenBlockModel, ConnectorCollection connectors)
        {   
            this.domenBlockModel = domenBlockModel;
            Position = new Point(domenBlockModel.Position.X, domenBlockModel.Position.Y);
            Content = new TextBlock();
            Width = domenBlockModel.Size.Width;
            Height = domenBlockModel.Size.Height;
            Connectors = connectors;
        }

        public void Refresh()
        {
            if (IsValid != DomenBlockModel.IsValid)
                IsValid = DomenBlockModel.IsValid;
            if (IsActive != DomenBlockModel.IsActive)
                IsActive = DomenBlockModel.IsActive;
            if (Caption != DomenBlockModel.Caption)
                Caption = DomenBlockModel.Caption;
            if (Position != DomenBlockModel.Position)
                Position = DomenBlockModel.Position;
            if (Width != DomenBlockModel.Size.Width)
                Width = DomenBlockModel.Size.Width;
            if (Height != DomenBlockModel.Size.Height)
                Height = DomenBlockModel.Size.Height;
            ToolTip = IsValid ? null : string.Join(Environment.NewLine, DomenBlockModel.StructureValidationResults.OfType<GraphItemValidationError>().Select(x => x.Message));
        }

        public void UpdateProtocol(ImmutableJson json)
        {
            DomenBlockModel.UpdateProtocol(json);
        }
    }
}