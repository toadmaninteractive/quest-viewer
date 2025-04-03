using QuestGraph.Core.Protocol;
using System;
using Telerik.Windows.Controls.Diagrams.Extensions.ViewModels;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core.TelerikDrawingModel
{
    public class Connection : LinkViewModelBase<NodeViewModelBase>
    {
        public string Id { get; private set; }
        public EdgeType Type => domenModel.Type;
        public CapType TargetCapTypeConnection { get; set; }
        public CapType SourceCapTypeConnection { get; set; }
        public string StrokeDashArrayConnection { get; set; }
        public string SourceConnectorPosition { get; set; } = "Auto";
        public string TargetConnectorPosition { get; set; } = "Auto";
        public string From => domenModel.From;
        public string To => domenModel.To;
        public int IndexInDomenList => domenModel.IndexInDomenList;

        private DomenModel.Edge domenModel;

        public Connection(DomenModel.Edge domenModel)
        {
            this.domenModel = domenModel;
            SetType(Type);
            Id = domenModel.RefId;
        }

        public void SwitchType()
        {
            domenModel.SwitchType();
            SetType(Type);
        }

        private void SetType(EdgeType type)
        {   
            switch (type)
            {
                case EdgeType.Unidirectional:
                    SourceCapTypeConnection = CapType.None;
                    TargetCapTypeConnection = CapType.Arrow3;
                    StrokeDashArrayConnection = null;
                    break;
                case EdgeType.Bidirectional:
                    switch (domenModel.TargetBlockType)
                    {
                        case NodeType.State:
                            SourceCapTypeConnection = CapType.Arrow3;
                            TargetCapTypeConnection = CapType.None;
                            break;
                        case NodeType.Action:
                            SourceCapTypeConnection = CapType.None;
                            TargetCapTypeConnection = CapType.Arrow3;
                            break;
                        default:
                            throw ExceptionConstructor.UnexpectedEnumValue(domenModel.TargetBlockType, Environment.StackTrace);
                    }
                    StrokeDashArrayConnection = "2 2";
                    break;
                default:
                    throw ExceptionConstructor.UnexpectedEnumValue(type, Environment.StackTrace);
            }

            RaisePropertyChanged(nameof(SourceCapTypeConnection));
            RaisePropertyChanged(nameof(StrokeDashArrayConnection));
        }

        public void UpdateConnectorPosition(string sourceConnectorPosition, string targetConnectorPosition)
        {
            SourceConnectorPosition = sourceConnectorPosition;
            TargetConnectorPosition = targetConnectorPosition;

            RaisePropertyChanged(nameof(SourceConnectorPosition));
            RaisePropertyChanged(nameof(TargetConnectorPosition));
        }

        public override string ToString()
        {
            return null;
        }
    }
}