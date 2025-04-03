using Json.Serialization;
using System.Collections.Generic;
using System.IO;

namespace QuestGraph.Core
{
    public abstract class ToFileSaver<T>
    {
        protected void Save(string folderAbsoluteName, string fileName, List<T> objectToSave, IJsonSerializer<T> jsonSerializerInstance)
        {
            if (!Directory.Exists(folderAbsoluteName))
                Directory.CreateDirectory(folderAbsoluteName);
            var json = JsonSerializer.List(jsonSerializerInstance).Serialize(objectToSave);
            File.WriteAllText(Path.Combine(folderAbsoluteName, fileName), json.ToString());
        }
    }
}