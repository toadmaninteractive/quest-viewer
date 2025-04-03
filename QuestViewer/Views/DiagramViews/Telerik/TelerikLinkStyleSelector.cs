using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QuestViewer.TelerikDrawing
{
    public class TelerikLinkStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            return (Style)Application.Current.FindResource("ConnectionStyle");
        }
    }
}