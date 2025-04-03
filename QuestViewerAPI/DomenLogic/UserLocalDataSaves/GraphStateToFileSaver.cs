using QuestGraph.Core.Protocol;
using System.Collections.Generic;
using System.IO;

namespace QuestGraph.Core
{
    public class GraphStateToFileSaver : ToFileSaver<GraphPreset>
    {
        public GraphStateToFileSaver()
        { }

        public void Save(string localDataFolderPath, List<GraphPreset> objectToSave)
        {
            var fileName = $"{Constants.GraphCustomPresetsFileName}{Constants.LocalStorageFileExtension}";
            Save(localDataFolderPath, fileName, objectToSave, GraphPresetJsonSerializer.Instance);
        }
    }
}