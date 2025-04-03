using System;
using System.Collections.Generic;

namespace QuestGraph.Core
{
    public interface IInterProcessCommunication
    {
        /// <summary>
        /// block diff states, documentId, database name
        /// </summary>
        event Action<BlockStateUpdateArg> OnStatesUpdateIPC;

        string ActualDatabase { get; }
        string ActualDocumentId { get; }
        GameLevelMonitor ExternalClientLevelMonitor { get; }
        Dictionary<string, bool> GetStates();
        void ApplyActiveBlocksWithoutResponseToExternalClient(Dictionary<string, bool> activeBlocks);
        void SetStateFromExternalClient(Dictionary<string, bool> activeBlocks);
    }
}