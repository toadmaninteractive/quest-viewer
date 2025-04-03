using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestViewer
{
    public class ValidationErrorsModel
    {
        public enum ErrorShowingMode { ShowIfHasError, ShowIfHasErrorAndFocus }

        /// <summary>
        /// bool param is force focus flag
        /// </summary>
        public event Action<ErrorShowingMode> OnChanged;
        public event Action<string> OnLostFocus;
        public int ErrorsCount => errors.Count(x => x.Value.NotificationType == UserNotificationType.Error);

        private Dictionary<string, UserNotification> errors { get; set; } = new Dictionary<string, UserNotification>();

        public UserNotification this[string key]
        {
            get { return errors[key]; }
            set { errors[key] = value; }
        }

        public void Remove(string key)
        {
            errors.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return errors.ContainsKey(key);
        }

        public void Changed(ErrorShowingMode forceFocus = ErrorShowingMode.ShowIfHasErrorAndFocus)
        {
            App.Current.Dispatcher.Invoke(() => OnChanged?.Invoke(forceFocus));
        }

        public void LostFocusRaise(string controlId)
        {
            App.Current.Dispatcher.Invoke(() => OnLostFocus?.Invoke(controlId));
        }
    }
}