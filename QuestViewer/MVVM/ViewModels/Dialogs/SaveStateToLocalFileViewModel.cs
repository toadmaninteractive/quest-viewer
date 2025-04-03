using System.Collections.Generic;
using System.Windows.Input;

namespace QuestViewer
{
    public class SaveStateToLocalFileViewModel : ValidationErrorsViewModel
    {
        public ICommand SaveCommand { get; protected set; }

        public string StateName
        {
            get { return stateName; }
            set
            {
                SetField(ref stateName, value);
                StateNameValidation();
            }
        }

        private List<string> existingStateNames;

        private string stateName;

        public SaveStateToLocalFileViewModel(List<string> existingStateNames)
        {
            Errors.OnLostFocus += LostFocusHandler;

            SaveCommand = new RelayCommand(SaveHandler, () => Errors.ErrorsCount == 0);
            this.existingStateNames = existingStateNames;
            EmptyValidation(nameof(StateName), StateName, Texts.View.SaveStateDialog.NameIsEmpty, UserNotificationType.Error);
        }

        private void SaveHandler()
        {
            StateNameValidation();
            if (Errors.ErrorsCount != 0)
                return;

            DialogResult = true;
        }

        private void LostFocusHandler(string controlId)
        {
            switch (controlId)
            {
                case nameof(StateName):
                    StateNameValidation();
                    break;
            }
        }

        private void StateNameValidation()
        {
            var isValid = EmptyValidation(nameof(StateName), StateName, Texts.View.SaveStateDialog.NameIsEmpty, UserNotificationType.Error);
            if (isValid)
                DuplicateValidation(existingStateNames, nameof(StateName), StateName, Texts.View.SaveStateDialog.NameIsExist, UserNotificationType.Warning);
            Errors.Changed();
        }
    }
}