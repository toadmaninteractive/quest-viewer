namespace QuestGraph.Core
{
    public abstract class PresetDropDown
    {
        /// <summary>
        /// unique preset name
        /// </summary>
        public string Name;
        public string Caption;

        public PresetDropDown(string name, string caption)
        {
            Name = name;
            Caption = caption;
        }

        public override string ToString()
        {
            return Caption;
        }
    }
}