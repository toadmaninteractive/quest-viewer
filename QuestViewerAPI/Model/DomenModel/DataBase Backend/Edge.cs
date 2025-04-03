using QuestGraph.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core.DomenModel
{
    public class Edge
    {
        public string RefId => source.RefId;
        public string From => source.From;
        public string To => source.To;
        public EdgeType Type => source.Type;
        public NodeType SourceBlockType => sourceBlockType;
        public NodeType TargetBlockType => targetBlockType;
        public bool IsValid => !StructureValidationResults.OfType<GraphItemValidationError>().Any();
        public int IndexInDomenList { get; }
        public List<GraphItemValidationResult> StructureValidationResults { get; private set; } = new List<GraphItemValidationResult>();

        private Protocol.Edge source;
        private NodeType sourceBlockType;
        private NodeType targetBlockType;

        public Edge(Protocol.Edge source, int index)
        {
            this.source = source;
            IndexInDomenList = index;
        }

        public void SwitchType()
        {
            switch (Type)
            {
                case EdgeType.Unidirectional:
                    source.Type = EdgeType.Bidirectional;
                    break;
                case EdgeType.Bidirectional:
                    source.Type = EdgeType.Unidirectional;
                    break;
                default:
                    throw ExceptionConstructor.UnexpectedEnumValue(Type, Environment.StackTrace);
            }
        }

        public void AppendValidationResult(GraphItemValidationResult error)
        {
            StructureValidationResults.Add(error);
        }

        public void SetBlockTypes(NodeType sourceBlockType, NodeType targetBlockType)
        {
            this.sourceBlockType = sourceBlockType;
            this.targetBlockType = targetBlockType;
        }
    }
}