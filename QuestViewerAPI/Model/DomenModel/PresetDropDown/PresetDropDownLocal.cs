using QuestGraph.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public class PresetDropDownLocal : PresetDropDown
    {
        public GraphPreset Preset { get; private set; }

        public PresetDropDownLocal(GraphPreset preset) : base(preset.Name, preset.Name)
        {
            Preset = preset;
        }
    }
}