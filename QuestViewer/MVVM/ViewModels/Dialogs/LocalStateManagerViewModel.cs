using QuestGraph.Core;
using QuestGraph.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuestViewer
{
    public class LocalStateManagerViewModel : DialogResultViewModel
    {
        public ICommand RemoveStateCommand { get; }
        public ICommand SaveStateCommand { get; }

        public ObservableCollection<GraphPreset> LocalStates {  get; }
        public GraphPreset SelectedState
        {
            get { return selectedState; }
            set { SetField(ref selectedState, value); }
        }

        private string userSavesFolderName;
        private Dictionary<string, bool> actualState;
        private Window windowOwner;
        private GraphPreset selectedState;

        public LocalStateManagerViewModel(string userSavesFolderName, Dictionary<string, bool> actualState, Window windowOwner)
        {
            this.userSavesFolderName = userSavesFolderName;
            var graphLocalStates = new GraphStateFromFileLoader().Load(userSavesFolderName);
            LocalStates = new ObservableCollection<GraphPreset>(graphLocalStates);

            this.actualState = actualState;
            this.windowOwner = windowOwner;

            RemoveStateCommand = new RelayCommand(RemoveStateHandler, () => SelectedState != null);
            SaveStateCommand = new RelayCommand(SaveStateHandler);
        }

        public void SaveStateHandler()
        {
            var saveStateRequest = new SaveStateToLocalFileViewModel(LocalStates.Select(x => x.Name).ToList());
            var saveStateRequestView = new SaveStateToLocalFile();
            saveStateRequestView.DataContext = saveStateRequest;
            saveStateRequestView.Owner = windowOwner;
            if (saveStateRequestView.ShowDialog() ?? false)
            {
                var state = LocalStates.SingleOrDefault(x => x.Name == saveStateRequest.StateName);
                if (state == null)
                {
                    state = new GraphPreset();
                    LocalStates.Add(state);
                }
                state.NodeState = actualState;
                state.Name = saveStateRequest.StateName;
                state.Description = string.Empty;
                new GraphStateToFileSaver().Save(userSavesFolderName, LocalStates.ToList());
            }
        }

        private void RemoveStateHandler()
        {
            var confirmationViewModel = new ConfirmationViewModel(Texts.View.LocalStateManagerDialog.RemoveConfirmation);
            var confirmationView = new ConfirmationView();
            confirmationView.Owner = windowOwner;
            confirmationView.DataContext = confirmationViewModel;
            var removedPressetIndex = LocalStates.ToList().FindIndex(x => x.Name == SelectedState.Name);
            if (confirmationView.ShowDialog() ?? false)
            {
                LocalStates.RemoveAt(removedPressetIndex);
                new GraphStateToFileSaver().Save(userSavesFolderName, LocalStates.ToList());
                LocalStates.Remove(SelectedState);
            }
        }
    }
}