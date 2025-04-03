using QuestGraph.Core;
using QuestGraph.Core.Protocol;
using QuestGraph.Core.TelerikDrawingModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace QuestViewer.TelerikDrawing
{
    public class TelerikBlockStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var blockViewModel = item as BlockBase;
            if (blockViewModel != null)
                return SelectStyle(blockViewModel.Type);
            else
            {
                var groupViewModel = item as BlockGroup;
                if (groupViewModel != null)
                    return (Style)Application.Current.FindResource("BlockGroupShapeStyle");
            }
            return base.SelectStyle(item, container);
        }

        private Style SelectStyle(NodeType blockType)
        {
            Style resultStyle;
            switch (blockType)
            {
                case NodeType.State:
                    resultStyle = (Style)Application.Current.FindResource("BlockStateStyle");
                    break;
                case NodeType.Action:
                    resultStyle = (Style)Application.Current.FindResource("BlockActionStyle");
                    break;
                default:
                    throw ExceptionConstructor.UnexpectedEnumValue(blockType, Environment.StackTrace);
            }

            return resultStyle;
        }
    }
}