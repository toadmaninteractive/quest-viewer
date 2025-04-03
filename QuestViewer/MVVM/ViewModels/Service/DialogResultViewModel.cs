namespace QuestViewer
{
    public abstract class DialogResultViewModel : NotifyPropertyChanged
    {
        public bool? DialogResult
        {
            get { return dialogResult; }
            set { SetField(ref dialogResult, value); }
        }

        private bool? dialogResult;
    }
}