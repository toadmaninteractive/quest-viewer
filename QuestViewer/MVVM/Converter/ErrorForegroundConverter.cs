using QuestGraph.Core;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuestViewer
{
    public class ErrorForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notification = (UserNotificationType)value;
            switch (notification)
            {
                case UserNotificationType.Error:
                    return Brushes.Firebrick;
                case UserNotificationType.Warning:
                    return Brushes.Orange;
                case UserNotificationType.Trace:
                    return Brushes.Gray;
                default:
                    throw ExceptionConstructor.UnexpectedEnumValue(notification, Environment.StackTrace);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}