using System.Collections.Generic;

namespace QuestViewer
{
    public abstract class ValidationErrorsViewModel : DialogResultViewModel
    {
        public ValidationErrorsModel Errors
        {
            get { return errors; }
            set { SetField(ref errors, value); }
        }

        private ValidationErrorsModel errors;

        public ValidationErrorsViewModel()
        {
            Errors = new ValidationErrorsModel();
        }

        protected bool DuplicateValidation(List<string> existingItems, string targetPropertyName, string propertyValue, string messageText, UserNotificationType notificationType)
        {
            if (existingItems.Contains(propertyValue.Trim()))
            {
                Errors[targetPropertyName] = new UserNotification(messageText, notificationType);
                return false;
            }
            else
            {
                Errors.Remove(targetPropertyName);
                return true;
            }
        }

        protected bool EmptyValidation(string targetPropertyName, string propertyValue, string messageText, UserNotificationType notificationType)
        {
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                Errors[targetPropertyName] = new UserNotification(messageText, notificationType);
                return false;
            }
            else
            {
                Errors.Remove(targetPropertyName);
                return true;
            }
        }
    }
}