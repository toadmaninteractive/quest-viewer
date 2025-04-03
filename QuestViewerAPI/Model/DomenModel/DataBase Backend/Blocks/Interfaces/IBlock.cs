using Json;
using System.Collections.Generic;
using System.Windows;

namespace QuestGraph.Core.DomenModel
{
    public interface IBlock
    {
        string Id { get; }
        bool IsActive { get; }
        string Caption { get; }
        Point Position { get; }
        Point Center { get; }
        Size Size { get; }
        Protocol.NodeType Type { get; }
        bool IsValid { get; }
        int IndexInDomenList { get; }
        List<GraphItemValidationResult> StructureValidationResults { get; }
        void Interact();
        void UpdateLayoutPosition(double x, double y);
        void UpdateLayoutSize(Size size);
        object GetDrawingModel();
        void AppendValidationResult(GraphItemValidationResult error);
        void UpdateProtocol(ImmutableJson json);
    }
}