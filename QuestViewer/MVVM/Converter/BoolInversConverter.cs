using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace QuestViewer
{
    public class BoolInversConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}