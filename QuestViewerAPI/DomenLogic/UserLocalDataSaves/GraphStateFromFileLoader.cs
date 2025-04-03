using QuestGraph.Core.Protocol;
using System.Collections.Generic;
using System.IO;

namespace QuestGraph.Core
{
    public class GraphStateFromFileLoader : FromFileLoader<GraphPreset>
    {
        public GraphStateFromFileLoader()
        { }

        public List<GraphPreset> Load(string localDataFolderPath)
        {
            var filePath = $"{Path.Combine(localDataFolderPath, Constants.GraphCustomPresetsFileName)}{Constants.LocalStorageFileExtension}";
            return Load(filePath, GraphPresetJsonSerializer.Instance);
        }
    }
}