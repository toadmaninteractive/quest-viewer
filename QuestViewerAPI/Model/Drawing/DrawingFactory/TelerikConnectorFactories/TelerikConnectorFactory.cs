using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace QuestGraph.Core
{
    public abstract class TelerikConnectorFactory
    {
        protected Visibility GetConnectorVisibility(BlockInteractionMode interactionMode)
        {
            switch (interactionMode)
            {
                case BlockInteractionMode.Constructor:
                    return Visibility.Visible;
                case BlockInteractionMode.Viewer:
                    return Visibility.Collapsed;
                default:
                    throw ExceptionConstructor.UnexpectedEnumValue(interactionMode, Environment.StackTrace);
            }
        }
    }
}