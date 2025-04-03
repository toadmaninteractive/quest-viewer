using Json;
using Json.Serialization;
using System.Collections.Generic;
using System.IO;

namespace QuestGraph.Core
{
    public abstract class FromFileLoader<T>
    {
        protected List<T> Load(string filePath, IJsonSerializer<T> jsonSerializerInstance)
        {
            if (File.Exists(filePath))
            {
                var presetsValue = File.ReadAllText(filePath);
                var presetsJsonValue = JsonParser.Parse(presetsValue);
                return JsonSerializer.List(jsonSerializerInstance).Deserialize(presetsJsonValue);
            }
            else
                return new List<T>();
        }
    }
}