using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public class UserNotification
    {
        public string NotificationText { get; }
        public UserNotificationType NotificationType { get; }

        public UserNotification(string text, UserNotificationType notificationType)
        {
            NotificationText = text;
            NotificationType = notificationType;
        }
    }
}