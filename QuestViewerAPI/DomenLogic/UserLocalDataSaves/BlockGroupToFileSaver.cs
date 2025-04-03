using QuestGraph.Core.Protocol;
using System.Collections.Generic;
using System.IO;

namespace QuestGraph.Core
{
    public class BlockGroupToFileSaver : ToFileSaver<NodeGroup>
    {
        public void Save(string localDataFolderPath, List<NodeGroup> objectToSave)
        {
            var fileName = $"{Constants.BlockGroupFileName}{Constants.LocalStorageFileExtension}";
            Save(localDataFolderPath, fileName, objectToSave, NodeGroupJsonSerializer.Instance);
        }
    }
}