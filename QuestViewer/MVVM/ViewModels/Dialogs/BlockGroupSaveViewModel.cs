using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuestViewer
{
    public class BlockGroupSaveViewModel : ValidationErrorsViewModel
    {
        public ICommand SaveCommand { get; protected set; }

		public string GroupName
        {
			get { return groupName; }
			set 
			{
                SetField(ref groupName, value);
                GroupNameValidation();
            }
		}

        private string groupName;
        private List<string> existingGroupNames;

        public BlockGroupSaveViewModel(List<string> existingGroupNames)
        {
            Errors.OnLostFocus += LostFocusHandler;

            this.existingGroupNames = existingGroupNames;
            SaveCommand = new RelayCommand(SaveHandler, () => Errors.ErrorsCount == 0);
            EmptyValidation(nameof(GroupName), GroupName, QuestGraph.Core.Texts.BlockGroupSaveDialog.GroupNameIsEmpty, UserNotificationType.Error);
        }

        private void SaveHandler()
        {
            GroupNameValidation();
            if (Errors.ErrorsCount != 0)
                return;

            DialogResult = true;
        }

        private void LostFocusHandler(string controlId)
        {
            switch (controlId)
            {
                case nameof(GroupName):
                    GroupNameValidation();
                    break;
            }
        }

        private void GroupNameValidation()
        {
            var isValid = EmptyValidation(nameof(GroupName), GroupName, Texts.View.SaveStateDialog.NameIsEmpty, UserNotificationType.Error);
            if (isValid)
                DuplicateValidation(existingGroupNames, nameof(GroupName), GroupName, QuestGraph.Core.Texts.BlockGroupSaveDialog.GroupNameIsExist, UserNotificationType.Error);
            Errors.Changed();
        }
    }
}