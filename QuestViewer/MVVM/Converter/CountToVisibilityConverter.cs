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
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
                return (int)value == 0 ? Visibility.Collapsed : Visibility.Visible;
            else if (value is double)
                return (double)value == 0 ? Visibility.Collapsed : Visibility.Visible;
            else if (value is decimal)
                return (decimal)value == 0 ? Visibility.Collapsed : Visibility.Visible;
            else if (value is long)
                return (long)value == 0 ? Visibility.Collapsed : Visibility.Visible;

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}