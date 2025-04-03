using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public static class Texts
    {
        public const string InvalidOperationCaption = "Invalid operation";

        public static class View
        {
            public static class QuestViewer
            {
                public static class ToolTips
                {
                    public static string NewConnections { get; set; } = "New Connection...";
                    public static string ConnectionManager { get; set; } = "Manage Connections...";
                    public static string Connections { get; set; } = "Open Connection";
                    public static string Graphs { get; set; } = "Graphs";
                    public static string GraphStates { get; set; } = "Presets";
                    public static string BlockGroupCollapse { get; set; } = "Expand/Collapse Selected Group";
                    public static string MapDiagram { get; set; } = "Center View";
                    public static string ConnectionRouting { get; set; } = "Smooth Edges";
                    public static string Grid { get; set; } = "Show Grid";
                    public static string BlockInteractionMode { get; set; } = "Interaction Mode";
                    public static string PresetManager { get; set; } = "Manage Presets...";
                    public static string Group { get; set; } = "Group selected Blocks";
                    public static string Ungroup { get; set; } = "Ungroup selected Group";
                    public static string FindGroups { get; set; } = "Find all Groups";
                }
            }
        }

        public static class CustomExceptionErrors
        {
            public const string UnexpectedEnumValue = "Unexpected value {0} for {1} enum";
        }

        public static class GraphStructureValidator
        {
            public const string EdgeIsFoating = "Floating edge {0}";
            public const string EdgeHasDuplicates = "Duplicate edge ID {0}";
            public const string EdgeAmbiguousConnection = "Edge {0} is connected to the node {1} having duplicate IDs";
            public const string EdgeCircularConnection = "Edge {0} is circularly connected to the same node {1}";

            public const string NodeHasDuplicates = "Duplicate node ID {0}";
            public const string NodeHasEmptyId = "Empty node ID {0}";
            public const string NodeIsFloating = "Floating node {0}";
            public const string NodeAmbiguousConnection = "Nodes {0} and {1} of the same type {2} are connected";

            public static class Captions
            {
                public const string EdgeIsFoating = "Floating edge";
                public const string EdgeHasDuplicates = "Duplicate edge ID";
                public const string EdgeAmbiguousConnection = "Duplicate node IDs connect";
                public const string EdgeCircularConnection = "Circularly connecting";

                public const string NodeHasDuplicates = "Duplicate node ID";
                public const string NodeHasEmptyId = "Empty node ID";
                public const string NodeIsFloating = "Floating node";
                public const string NodeAmbiguousConnection = "Same type connect";
            }
        }

        public static class BlockGroupSaveDialog
        {
            public static string Title { get; set; } = "New Group";
            public static string GroupName { get; set; } = "Group Name";
            public static string GroupNameIsEmpty { get; set; } = "Group name is not set";
            public static string GroupNameIsExist { get; set; } = "Group with the same name already exists";
        }

        public static class BlockGroupsValidator
        {
            public const string AlreadyGrouped = "Some blocks in selection are already in another group";
        }

        public static class GraphBuilder
        {
            public const string BlockIdIntersect = "Domen model having intersect block IDs";
        }
    }
}