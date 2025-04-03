using QuestGraph.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuestViewer
{
    public class GraphStateLoaderViewModel : DialogResultViewModel
    {
        public ICommand LoadCommand { get; set; }

        public PresetDropDown SelectedState
        {
            get { return selectedState; }
            set { SetField(ref selectedState, value); }
        }

        public List<PresetDropDown> ExistingStates
        {
            get { return existingStates; }
            set { SetField(ref existingStates, value); }
        }

        private PresetDropDown selectedState;
        private List<PresetDropDown> existingStates;

        public GraphStateLoaderViewModel(List<PresetDropDown> existingStates) 
        {
            LoadCommand = new RelayCommand(LoadHandler, () => SelectedState != null);

            ExistingStates = existingStates;
            SelectedState = existingStates.FirstOrDefault();
        }

        private void LoadHandler()
        {
            if (SelectedState != null)
                DialogResult = true;
        }
    }
}