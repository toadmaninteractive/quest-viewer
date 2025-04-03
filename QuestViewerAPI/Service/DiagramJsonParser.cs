using Json;
using Json.Serialization;
using QuestGraph.Core.Protocol;
using System.Collections.Generic;

namespace QuestGraph.Core
{

    public sealed class EdgeTypeJsonSerializer : Json.Serialization.IJsonSerializer<EdgeType>, Json.Serialization.IJsonKeySerializer<EdgeType>
    {
        public static readonly EdgeTypeJsonSerializer Instance = new EdgeTypeJsonSerializer();

        public Json.ImmutableJson Serialize(EdgeType value)
        {
            return SerializeKey(value);
        }

        public EdgeType Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return DeserializeKey(json.AsString);
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsString;
        }

        public string SerializeKey(EdgeType value)
        {
            switch (value)
            {
                case EdgeType.Unidirectional: return "unidirectional";
                case EdgeType.Bidirectional: return "bidirectional";
                default: throw new System.ArgumentOutOfRangeException(nameof(value));
            }
        }

        public EdgeType DeserializeKey(string jsonKey)
        {
            switch (jsonKey)
            {
                case "unidirectional": return EdgeType.Unidirectional;
                case "bidirectional": return EdgeType.Bidirectional;
                default: throw new System.ArgumentOutOfRangeException(nameof(jsonKey));
            }
        }
    }

    public sealed class EdgeJsonSerializer : Json.Serialization.IJsonSerializer<Edge>
    {
        public static readonly EdgeJsonSerializer Instance = new EdgeJsonSerializer();

        public Json.ImmutableJson Serialize(Edge value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            if (value.RefId == null)
                throw new System.ArgumentException("Required property RefId is null", nameof(value));
            if (value.From == null)
                throw new System.ArgumentException("Required property From is null", nameof(value));
            if (value.To == null)
                throw new System.ArgumentException("Required property To is null", nameof(value));

            return new Json.JsonObject
            {
                ["ref_id"] = JsonSerializer.String.Serialize(value.RefId),
                ["from"] = JsonSerializer.String.Serialize(value.From),
                ["to"] = JsonSerializer.String.Serialize(value.To),
                ["type"] = EdgeTypeJsonSerializer.Instance.Serialize(value.Type)
            };
        }

        public Edge Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            var result = new Edge();
            Deserialize(json, result);
            return result;
        }

        public void Deserialize(Json.ImmutableJson json, Edge value)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            value.RefId = JsonSerializer.String.Deserialize(json["ref_id"]);
            value.From = JsonSerializer.String.Deserialize(json["from"]);
            value.To = JsonSerializer.String.Deserialize(json["to"]);
            value.Type = EdgeTypeJsonSerializer.Instance.Deserialize(json["type"]);
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsObject;
        }
    }

    public sealed class NodeTypeJsonSerializer : Json.Serialization.IJsonSerializer<NodeType>, Json.Serialization.IJsonKeySerializer<NodeType>
    {
        public static readonly NodeTypeJsonSerializer Instance = new NodeTypeJsonSerializer();

        public Json.ImmutableJson Serialize(NodeType value)
        {
            return SerializeKey(value);
        }

        public NodeType Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return DeserializeKey(json.AsString);
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsString;
        }

        public string SerializeKey(NodeType value)
        {
            switch (value)
            {
                case NodeType.State: return "state";
                case NodeType.Action: return "action";
                default: throw new System.ArgumentOutOfRangeException(nameof(value));
            }
        }

        public NodeType DeserializeKey(string jsonKey)
        {
            switch (jsonKey)
            {
                case "state": return NodeType.State;
                case "action": return NodeType.Action;
                default: throw new System.ArgumentOutOfRangeException(nameof(jsonKey));
            }
        }
    }

    public sealed class LayoutJsonSerializer : Json.Serialization.IJsonSerializer<Layout>
    {
        public static readonly LayoutJsonSerializer Instance = new LayoutJsonSerializer();

        public Json.ImmutableJson Serialize(Layout value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            return new Json.JsonObject
            {
                ["x"] = JsonSerializer.Double.Serialize(value.X),
                ["y"] = JsonSerializer.Double.Serialize(value.Y),
                ["width"] = JsonSerializer.Double.Serialize(value.Width),
                ["height"] = JsonSerializer.Double.Serialize(value.Height)
            };
        }

        public Layout Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            var result = new Layout();
            Deserialize(json, result);
            return result;
        }

        public void Deserialize(Json.ImmutableJson json, Layout value)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            value.X = JsonSerializer.Double.Deserialize(json["x"]);
            value.Y = JsonSerializer.Double.Deserialize(json["y"]);
            value.Width = JsonSerializer.Double.Deserialize(json["width"]);
            value.Height = JsonSerializer.Double.Deserialize(json["height"]);
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsObject;
        }
    }

    public sealed class NodeJsonSerializer : Json.Serialization.IJsonSerializer<Node>
    {
        public static readonly NodeJsonSerializer Instance = new NodeJsonSerializer();

        public Json.ImmutableJson Serialize(Node value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            switch (value.Type)
            {
                case NodeType.State: return NodeStateJsonSerializer.Instance.Serialize((NodeState)value);
                case NodeType.Action: return NodeActionJsonSerializer.Instance.Serialize((NodeAction)value);
                default: throw new System.ArgumentException("Invalid variant tag");
            }
        }

        public Node Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            NodeType type = NodeTypeJsonSerializer.Instance.Deserialize(json["type"]);
            switch (type)
            {
                case NodeType.State: return NodeStateJsonSerializer.Instance.Deserialize(json);
                case NodeType.Action: return NodeActionJsonSerializer.Instance.Deserialize(json);
                default: throw new System.ArgumentException("Invalid variant tag");
            }
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsObject && json.AsObject.ContainsKey("type");
        }
    }

    public sealed class NodeStateJsonSerializer : Json.Serialization.IJsonSerializer<NodeState>
    {
        public static readonly NodeStateJsonSerializer Instance = new NodeStateJsonSerializer();

        public Json.ImmutableJson Serialize(NodeState value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            if (value.RefId == null)
                throw new System.ArgumentException("Required property RefId is null", nameof(value));
            if (value.Name == null)
                throw new System.ArgumentException("Required property Name is null", nameof(value));
            if (value.Description == null)
                throw new System.ArgumentException("Required property Description is null", nameof(value));
            if (value.Layout == null)
                throw new System.ArgumentException("Required property Layout is null", nameof(value));

            return new Json.JsonObject
            {
                ["ref_id"] = JsonSerializer.String.Serialize(value.RefId),
                ["name"] = JsonSerializer.String.Serialize(value.Name),
                ["description"] = JsonSerializer.String.Serialize(value.Description),
                ["layout"] = LayoutJsonSerializer.Instance.Serialize(value.Layout),
                ["type"] = NodeTypeJsonSerializer.Instance.Serialize(value.Type),
                ["is_active"] = JsonSerializer.Bool.Serialize(value.IsActive)
            };
        }

        public NodeState Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            var result = new NodeState();
            Deserialize(json, result);
            return result;
        }

        public void Deserialize(Json.ImmutableJson json, NodeState value)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            value.RefId = JsonSerializer.String.Deserialize(json["ref_id"]);
            value.Name = JsonSerializer.String.Deserialize(json["name"]);
            value.Description = JsonSerializer.String.Deserialize(json["description"]);
            value.Layout = LayoutJsonSerializer.Instance.Deserialize(json["layout"]);
            value.IsActive = JsonSerializer.Bool.Deserialize(json["is_active"]);
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsObject;
        }
    }

    public sealed class NodeActionJsonSerializer : Json.Serialization.IJsonSerializer<NodeAction>
    {
        public static readonly NodeActionJsonSerializer Instance = new NodeActionJsonSerializer();

        public Json.ImmutableJson Serialize(NodeAction value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            if (value.RefId == null)
                throw new System.ArgumentException("Required property RefId is null", nameof(value));
            if (value.Name == null)
                throw new System.ArgumentException("Required property Name is null", nameof(value));
            if (value.Description == null)
                throw new System.ArgumentException("Required property Description is null", nameof(value));
            if (value.Layout == null)
                throw new System.ArgumentException("Required property Layout is null", nameof(value));

            return new Json.JsonObject
            {
                ["ref_id"] = JsonSerializer.String.Serialize(value.RefId),
                ["name"] = JsonSerializer.String.Serialize(value.Name),
                ["description"] = JsonSerializer.String.Serialize(value.Description),
                ["layout"] = LayoutJsonSerializer.Instance.Serialize(value.Layout),
                ["type"] = NodeTypeJsonSerializer.Instance.Serialize(value.Type)
            };
        }

        public NodeAction Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            var result = new NodeAction();
            Deserialize(json, result);
            return result;
        }

        public void Deserialize(Json.ImmutableJson json, NodeAction value)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            value.RefId = JsonSerializer.String.Deserialize(json["ref_id"]);
            value.Name = JsonSerializer.String.Deserialize(json["name"]);
            value.Description = JsonSerializer.String.Deserialize(json["description"]);
            value.Layout = LayoutJsonSerializer.Instance.Deserialize(json["layout"]);
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsObject;
        }
    }

    public sealed class GraphPresetJsonSerializer : Json.Serialization.IJsonSerializer<GraphPreset>
    {
        public static readonly GraphPresetJsonSerializer Instance = new GraphPresetJsonSerializer();
        public static readonly string PresetsName = "presets";

        public Json.ImmutableJson Serialize(GraphPreset value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            if (value.Name == null)
                throw new System.ArgumentException("Required property Name is null", nameof(value));
            if (value.Description == null)
                throw new System.ArgumentException("Required property Description is null", nameof(value));
            if (value.NodeState == null)
                throw new System.ArgumentException("Required property NodeState is null", nameof(value));
            return new Json.JsonObject
            {
                ["name"] = JsonSerializer.String.Serialize(value.Name),
                ["description"] = JsonSerializer.String.Serialize(value.Description),
                ["node_state"] = JsonSerializer.Dict(JsonSerializer.String, JsonSerializer.Bool).Serialize(value.NodeState)
            };
        }

        public GraphPreset Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            var result = new GraphPreset();
            Deserialize(json, result);
            return result;
        }

        public GraphPreset Deserialize(string json)
        {
            return Deserialize(JsonParser.Parse(json));
        }

        public void Deserialize(Json.ImmutableJson json, GraphPreset value)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            value.Name = JsonSerializer.String.Deserialize(json["name"]);
            value.Description = JsonSerializer.String.Deserialize(json["description"]);
            value.NodeState = JsonSerializer.Dict(JsonSerializer.String, JsonSerializer.Bool).Deserialize(json["node_state"]);
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsObject;
        }
    }

    public sealed class CardCategoryJsonSerializer : Json.Serialization.IJsonSerializer<CardCategory>, Json.Serialization.IJsonKeySerializer<CardCategory>
    {
        public static readonly CardCategoryJsonSerializer Instance = new CardCategoryJsonSerializer();

        public Json.ImmutableJson Serialize(CardCategory value)
        {
            return SerializeKey(value);
        }

        public CardCategory Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return DeserializeKey(json.AsString);
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsString;
        }

        public string SerializeKey(CardCategory value)
        {
            switch (value)
            {
                case CardCategory.QuestGraph: return "quest_graph";
                default: throw new System.ArgumentOutOfRangeException(nameof(value));
            }
        }

        public CardCategory DeserializeKey(string jsonKey)
        {
            switch (jsonKey)
            {
                case "quest_graph": return CardCategory.QuestGraph;
                default: throw new System.ArgumentOutOfRangeException(nameof(jsonKey));
            }
        }
    }

    public sealed class CardJsonSerializer : Json.Serialization.IJsonSerializer<Card>
    {
        public static readonly CardJsonSerializer Instance = new CardJsonSerializer();

        public Json.ImmutableJson Serialize(Card value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            switch (value.Category)
            {
                case CardCategory.QuestGraph: return CardQuestGraphJsonSerializer.Instance.Serialize((CardQuestGraph)value);
                default: throw new System.ArgumentException("Invalid variant tag");
            }
        }

        public Card Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            CardCategory category = CardCategoryJsonSerializer.Instance.Deserialize(json["category"]);
            switch (category)
            {
                case CardCategory.QuestGraph: return CardQuestGraphJsonSerializer.Instance.Deserialize(json);
                default: throw new System.ArgumentException("Invalid variant tag");
            }
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsObject && json.AsObject.ContainsKey("category");
        }
    }

    public sealed class CardQuestGraphJsonSerializer : Json.Serialization.IJsonSerializer<CardQuestGraph>
    {
        public static readonly CardQuestGraphJsonSerializer Instance = new CardQuestGraphJsonSerializer();

        public Json.ImmutableJson Serialize(CardQuestGraph value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            if (value.Id == null)
                throw new System.ArgumentException("Required property Id is null", nameof(value));
            if (value.Nodes == null)
                throw new System.ArgumentException("Required property Nodes is null", nameof(value));
            if (value.Edges == null)
                throw new System.ArgumentException("Required property Edges is null", nameof(value));
            if (value.Presets == null)
                throw new System.ArgumentException("Required property Presets is null", nameof(value));
            return new Json.JsonObject
            {
                ["category"] = CardCategoryJsonSerializer.Instance.Serialize(value.Category),
                ["_id"] = JsonSerializer.String.Serialize(value.Id),
                ["nodes"] = JsonSerializer.List(NodeJsonSerializer.Instance).Serialize(value.Nodes),
                ["edges"] = JsonSerializer.List(EdgeJsonSerializer.Instance).Serialize(value.Edges),
                ["presets"] = JsonSerializer.List(GraphPresetJsonSerializer.Instance).Serialize(value.Presets)
            };
        }

        public CardQuestGraph Deserialize(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            var result = new CardQuestGraph();
            Deserialize(json, result);
            return result;
        }

        public void Deserialize(Json.ImmutableJson json, CardQuestGraph value)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            value.Id = JsonSerializer.String.Deserialize(json["_id"]);
            value.Nodes = JsonSerializer.List(NodeJsonSerializer.Instance).Deserialize(json["nodes"]);
            value.Edges = JsonSerializer.List(EdgeJsonSerializer.Instance).Deserialize(json["edges"]);
            value.Presets = JsonSerializer.List(GraphPresetJsonSerializer.Instance).Deserialize(json["presets"]);
        }

        public CardQuestGraph DeserializeContentOnly(string json)
        {
            return DeserializeContentOnly(JsonParser.Parse(json));
        }

        public CardQuestGraph DeserializeContentOnly(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));

            var value = new CardQuestGraph();
            value.Nodes = JsonSerializer.List(NodeJsonSerializer.Instance).Deserialize(json["nodes"]);
            value.Edges = JsonSerializer.List(EdgeJsonSerializer.Instance).Deserialize(json["edges"]);
            value.Presets = JsonSerializer.List(GraphPresetJsonSerializer.Instance).Deserialize(json["presets"]);

            return value;
        }

        public bool Test(Json.ImmutableJson json)
        {
            if (json == null)
                throw new System.ArgumentNullException(nameof(json));
            return json.IsObject;
        }
    }
}