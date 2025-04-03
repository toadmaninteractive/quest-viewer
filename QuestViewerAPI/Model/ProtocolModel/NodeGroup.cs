using Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core.Protocol
{
    public sealed class NodeGroup
    {
        public string Name { get; set; }

        public Layout Layout { get; set; }

        public bool IsExpanded { get; set; }

        public List<string> Nodes { get; set; }
    }

    public sealed class NodeGroupJsonSerializer : Json.Serialization.IJsonSerializer<NodeGroup>
    {
        public static readonly NodeGroupJsonSerializer Instance = new NodeGroupJsonSerializer();

        public Json.ImmutableJson Serialize(NodeGroup value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            if (value.Name == null)
                throw new System.ArgumentException("Required property Name is null", nameof(value));
            if (value.Layout == null)
                throw new System.ArgumentException("Required property Layout is null", nameof(value));
            if (value.Nodes == null)
                throw new System.ArgumentException("Required property Nodes is null", nameof(value));
            return new Json.JsonObject
            {
                ["name"] = JsonSerializer.String.Serialize(value.Name),
                ["layout"] = LayoutJsonSerializer.Instance.Serialize(value.Layout),
                ["is_expanded"] = JsonSerializer.Bool.Serialize(value.IsExpanded),
                ["nodes"] = JsonSerializer.List(JsonSerializer.String).Serialize(value.Nodes)
            };
        }

        public NodeGroup Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            var result = new NodeGroup();
            Deserialize(json, result);
            return result;
        }

        public void Deserialize(Json.ImmutableJson json, NodeGroup value)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            value.Name = JsonSerializer.String.Deserialize(json["name"]);
            value.Layout = LayoutJsonSerializer.Instance.Deserialize(json["layout"]);
            value.IsExpanded = JsonSerializer.Bool.Deserialize(json["is_expanded"]);
            value.Nodes = JsonSerializer.List(JsonSerializer.String).Deserialize(json["nodes"]);
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsObject;
        }
    }
}