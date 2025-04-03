using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuestViewer
{
    public class ConfirmationViewModel : DialogResultViewModel
    {
        public ICommand OkCommand { get; set; }

        public string Text
        {
            get { return text; }
            set { SetField(ref text, value); }
        }

        private string text;

        public ConfirmationViewModel(string text)
        {
            this.text = text;
            OkCommand = new RelayCommand(() => DialogResult = true);
        }
    }
}