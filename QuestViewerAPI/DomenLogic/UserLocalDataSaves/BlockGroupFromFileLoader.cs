using QuestGraph.Core.Protocol;
using System.Collections.Generic;
using System.IO;

namespace QuestGraph.Core
{
    public class BlockGroupFromFileLoader : FromFileLoader<NodeGroup>
    {
        public List<NodeGroup> Load(string localDataFolderPath)
        {
            var filePath = $"{Path.Combine(localDataFolderPath, Constants.BlockGroupFileName)}{Constants.LocalStorageFileExtension}";
            return Load(filePath, NodeGroupJsonSerializer.Instance);
        }
    }
}