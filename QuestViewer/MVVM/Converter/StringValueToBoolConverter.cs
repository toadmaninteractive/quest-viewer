using System;
using System.Globalization;
using System.Windows.Data;

namespace QuestViewer
{
    public  class StringValueToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        public bool Convert(object value)
        {
            var strObj = value as string;
            if (strObj == null)
                return false;

            return !string.IsNullOrWhiteSpace(strObj);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}