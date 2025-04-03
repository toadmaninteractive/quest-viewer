using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace QuestViewer
{
    public class ComboBoxWatermarkVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return IsValueEmpty(values[0]) && IsValueEmpty(values[1]) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool IsValueEmpty(object value)
        {
            if (value is string strObj)
                return string.IsNullOrWhiteSpace(strObj);

            if (value is QuestGraph.Core.DomenModel.BlockGroup)
                return false;

            return false;
        }
    }
}