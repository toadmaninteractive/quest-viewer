using System.Collections.Generic;
using System.Windows.Input;
using static QuestViewer.ValidationErrorsModel;

namespace QuestViewer
{
    public class CdbConnectionNewViewModel : ValidationErrorsViewModel
    {
        public string Title
        {
            get { return title; }
            set
            {
                SetField(ref title, value);
                FieldValidation(nameof(Title));
            }
        }
        public string Url
        {
            get { return url; }
            set
            {
                SetField(ref url, value);
                FieldValidation(nameof(Url));
            }
        }
        public string Database
        {
            get { return database; }
            set
            {
                SetField(ref database, value);
                FieldValidation(nameof(Database));

                Title = value;
            }
        }
        public string Username
        {
            get { return username; }
            set
            {
                SetField(ref username, value);
                FieldValidation(nameof(Username));
            }
        }
        public List<string> ServerDataBases
        {
            get { return serverDataBases; }
            set { SetField(ref serverDataBases, value); }
        }

        public bool HasEmptyFields => string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Database) || string.IsNullOrEmpty(Title);


        private readonly List<string> existingTitles;
        private string title;
        private string url;
        private string database;
        private string username;
        private List<string> serverDataBases;

        public CdbConnectionNewViewModel(List<string> existingTitles)
        {
            Errors.OnLostFocus += FieldValidation;
            this.existingTitles = existingTitles;
        }

        public void FieldsValidation()
        {
            var isValid = EmptyValidation(nameof(Title), Title, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
            if (isValid)
                DuplicateValidation(existingTitles, nameof(Title), Title, Texts.View.ConnectionManager.TitleIsExist, UserNotificationType.Error);
            EmptyValidation(nameof(Url), Url, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
            EmptyValidation(nameof(Database), Database, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
            EmptyValidation(nameof(Username), Username, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
            Errors.Changed(ErrorShowingMode.ShowIfHasError);
        }

        public bool ServerRequestFieldsValidation()
        {
            return EmptyValidation(nameof(Url), Url, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error)
                && EmptyValidation(nameof(Username), Username, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
        }

        private void FieldValidation(string controlId)
        {
            switch (controlId)
            {
                case nameof(Title):
                    var isValid = EmptyValidation(controlId, Title, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
                    if (isValid)
                        DuplicateValidation(existingTitles, nameof(Title), Title, Texts.View.ConnectionManager.TitleIsExist, UserNotificationType.Error);
                    break;
                case nameof(Url):
                    EmptyValidation(controlId, Url, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
                    break;
                case nameof(Database):
                    EmptyValidation(controlId, Database, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
                    break;
                case nameof(Username):
                    EmptyValidation(controlId, Username, Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
                    break;
            }

            Errors.Changed();
        }
    }
}