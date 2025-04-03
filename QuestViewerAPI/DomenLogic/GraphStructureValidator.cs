using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public class GraphItemValidationWarning : GraphItemValidationResult
    {
        public GraphItemValidationWarning(string message, string caption, string itemRef, int position) 
            : base(message, caption, itemRef, position)
        {
        }
    }

    public class GraphItemValidationError : GraphItemValidationResult
    {
        public GraphItemValidationError(string message, string caption, string itemRef, int position)
            : base(message, caption, itemRef, position)
        {
        }
    }

    public abstract class GraphItemValidationResult
    {
        public string Caption { get; set; }
        public string Message { get; set; }
        public string ItemRef { get; set; }
        public int Position { get; set; }

        public GraphItemValidationResult(string message, string caption, string itemRef, int position)
        {
            Caption = caption;
            Message = $"[{position + 1}] {message}";
            ItemRef = itemRef;
            Position = position;
        }

        public override string ToString()
        {
            return $"[{Position + 1}] {Caption}";
        }
    }

    public class GraphStructureValidator
    {
        public GraphStructureValidator() { }

        public void GraphStructureValidation(List<DomenModel.IBlock> protocolBlocks, List<DomenModel.Edge> protocolEdge)
        {
            var edgeCounter = new Dictionary<string, int>();
            foreach (var edge in protocolEdge)
            {
                var position = protocolEdge.IndexOf(edge);

                if (edgeCounter.ContainsKey(edge.RefId))
                    edgeCounter[edge.RefId]++;
                else
                    edgeCounter[edge.RefId] = 1;

                var chainedBlocksFrom = protocolBlocks.Where(x => x.Id == edge.From).ToArray();
                var chainedBlocksTo = protocolBlocks.Where(x => x.Id == edge.To).ToArray();
                if (chainedBlocksFrom.Length < 1 || chainedBlocksTo.Length < 1)
                {
                    var message = string.Format(Texts.GraphStructureValidator.EdgeIsFoating, edge.RefId);
                    edge.AppendValidationResult(new GraphItemValidationError(message, Texts.GraphStructureValidator.Captions.EdgeIsFoating, edge.RefId, position));
                }
                else if (chainedBlocksFrom.Length > 1 || chainedBlocksTo.Length > 1)
                {
                    var blocks = chainedBlocksFrom.Concat(chainedBlocksTo);
                    var message = string.Format(Texts.GraphStructureValidator.EdgeAmbiguousConnection, edge.RefId, string.Join(", ", blocks));
                    edge.AppendValidationResult(new GraphItemValidationError(message, Texts.GraphStructureValidator.Captions.EdgeAmbiguousConnection, edge.RefId, position));
                }
                else if (chainedBlocksFrom[0].Id == chainedBlocksTo[0].Id)
                {
                    var message = string.Format(Texts.GraphStructureValidator.EdgeCircularConnection, edge.RefId, chainedBlocksFrom[0].Id);
                    edge.AppendValidationResult(new GraphItemValidationError(message, Texts.GraphStructureValidator.Captions.EdgeCircularConnection, edge.RefId, position));
                }
                else if (chainedBlocksFrom[0].Type == chainedBlocksTo[0].Type)
                {
                    var message = string.Format(Texts.GraphStructureValidator.NodeAmbiguousConnection, chainedBlocksTo[0].Id, chainedBlocksFrom[0].Id, edge.RefId);
                    edge.AppendValidationResult(new GraphItemValidationError(message, Texts.GraphStructureValidator.Captions.NodeAmbiguousConnection, edge.RefId, position));
                }
            }

            foreach (var duplicatedEdgeCounter in edgeCounter.Where(x => x.Value > 1))
            {
                var message = string.Format(Texts.GraphStructureValidator.EdgeHasDuplicates, duplicatedEdgeCounter.Key);
                foreach (var duplicatedEdge in protocolEdge.Where(x => x.RefId == duplicatedEdgeCounter.Key))
                {
                    var position = protocolEdge.IndexOf(duplicatedEdge);
                    duplicatedEdge.AppendValidationResult(new GraphItemValidationError(message, Texts.GraphStructureValidator.Captions.EdgeHasDuplicates, duplicatedEdgeCounter.Key, position));
                }
            }

            var blockCounter = new Dictionary<string, int>();
            foreach (var block in protocolBlocks)
            {
                var position = protocolBlocks.IndexOf(block);
                if (blockCounter.ContainsKey(block.Id))
                    blockCounter[block.Id]++;
                else
                    blockCounter[block.Id] = 1;

                if (protocolEdge.Count(x => x.From == block.Id || x.To == block.Id) == 0)
                {
                    var message = string.Format(Texts.GraphStructureValidator.NodeIsFloating, block.Id);
                    block.AppendValidationResult(new GraphItemValidationWarning(message, Texts.GraphStructureValidator.Captions.NodeIsFloating, block.Id, position));
                }

                if (string.IsNullOrWhiteSpace(block.Id))
                {
                    var message = string.Format(Texts.GraphStructureValidator.NodeHasEmptyId, block.Id);
                    block.AppendValidationResult(new GraphItemValidationError(message, Texts.GraphStructureValidator.Captions.NodeHasEmptyId, block.Id, position));
                }
            }

            foreach (var duplicatedBlockCounter in blockCounter.Where(x => x.Value > 1))
            {
                var message = string.Format(Texts.GraphStructureValidator.NodeHasDuplicates, duplicatedBlockCounter.Key);
                foreach (var block in protocolBlocks.Where(x => x.Id == duplicatedBlockCounter.Key))
                {
                    var position = protocolBlocks.IndexOf(block);
                    block.AppendValidationResult(new GraphItemValidationError(message, Texts.GraphStructureValidator.Captions.NodeHasDuplicates, duplicatedBlockCounter.Key, position));
                }
            }
        }
    }
}