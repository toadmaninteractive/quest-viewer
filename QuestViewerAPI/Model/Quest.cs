using Json;
using Microsoft.Msagl.Core.Routing;
using QuestGraph.Core.DomenModel;
using QuestGraph.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace QuestGraph.Core
{
    /// <summary>
    /// Domen logic class
    /// </summary>
    public class Quest : IGraphInteractive, IInterProcessCommunication
    {
        #region events for update QuestViewer window without bindings 
        public event Action OnStatesUpdate;
        public event Action OnBlockGroupCollectionChanged;
        public event Action<string, bool> OnBlockGroupExpandChanged;
        public event Action<IBlock> OnBlockPositionChanged;
        public event Action<IBlock> OnBlockSizeChanged;
        public event Action<DomenModel.BlockGroup> OnBlockGroupSizeChanged;
        #endregion

        #region Inter Process Communication
        public event Action<BlockStateUpdateArg> OnStatesUpdateIPC;
        #endregion

        public BlockInteractionMode InteractionMode => drawingFactory.DrawingConfig.InteractionMode;
        public IDrawingModelService DrawingModelService;
        public EdgesRoutingMode RoutingMode => drawingConfig.RoutingMode;
        public string ActualDatabase => actualDatabase;
        public string ActualDocumentId => actualDocumentId;
        public GameLevelMonitor ExternalClientLevelMonitor { get; } = new GameLevelMonitor();

        private List<DomenModel.Edge> edgesDomenCache;
        private List<IBlock> blockDomenCache;
        private List<BlockGroup> blockGroupDomenCache;
        private List<GraphPreset> presetsCache;
        private GraphPreset originDatabaseStateCache;
        private GraphBuilder graphBuilder = new GraphBuilder();

        private IDrawingFactory drawingFactory;
        private DrawingConfiguration drawingConfig;
        private string actualDatabase;
        private string actualDocumentId;

        public Quest(DrawingConfiguration drawingConfig) 
        {
            this.drawingConfig = drawingConfig;
            drawingFactory = new TelerikDrawingFactory(drawingConfig); //MsaglDrawingFactory(); TelerikDrawingFactory();
            DrawingModelService = new TelerikDrawingModelService();
        }

        #region Interactive

        public void InteractWithBlock(IBlock block)
        {
            switch (drawingFactory.DrawingConfig.InteractionMode)
            {
                case BlockInteractionMode.Constructor:
                    break;
                case BlockInteractionMode.Viewer:
                    var previosBlockStates = GetStates();
                    block.Interact();
                    OnStatesUpdate?.Invoke();
                    var blockStateDiff = GetDiffStates(previosBlockStates);
                    OnStatesUpdateIPC?.Invoke(new BlockStateUpdateArg(blockStateDiff, actualDocumentId, actualDatabase));
                    break;
                default:
                    throw ExceptionConstructor.UnexpectedEnumValue(drawingFactory.DrawingConfig, Environment.StackTrace);
            }
        }

        public void UpdateDomenModelBlockPosition(IBlock block, double x, double y)
        {
            block.UpdateLayoutPosition(x, y);
            OnBlockPositionChanged?.Invoke(block);
        }

        public void BlockSizeChanged(IBlock block, Size size)
        {
            block.UpdateLayoutSize(size);
            OnBlockSizeChanged?.Invoke(block);
        }

        public void BlockGroupSizeChanged(DomenModel.BlockGroup blockGroup, System.Windows.Size size)
        {
            blockGroup.UpdateLayoutSize(size);
            OnBlockGroupSizeChanged?.Invoke(blockGroup);
        }

        public void UpdateDrawingConfiguration(DrawingConfiguration drawingConfig)
        {
            this.drawingConfig = drawingConfig;
            drawingFactory = new TelerikDrawingFactory(drawingConfig);
        }

        public void UpdateDrawingConfiguration(EdgesRoutingMode edgesRoutingMode)
        {
            drawingConfig = new DrawingConfiguration(drawingConfig.InteractionMode, drawingConfig.ConnectorMode, edgesRoutingMode);
            drawingFactory = new TelerikDrawingFactory(drawingConfig);
        }

        public void UpdateDrawingConfiguration(BlockInteractionMode interactionMode)
        {
            drawingConfig = new DrawingConfiguration(interactionMode, drawingConfig.ConnectorMode, drawingConfig.RoutingMode);
            drawingFactory = new TelerikDrawingFactory(drawingConfig);
        }

        public void InteractWithBlockGroup(BlockGroup blockGroupDomenModel)
        {
            blockGroupDomenModel.ExpandedSwitch();
            var blockDomenModels = blockDomenCache.Where(x => blockGroupDomenModel.Nodes.Contains(x.Id));
            var blocksCenter = GetCenter(blockDomenModels.Select(x => x.Center));
            if (blockGroupDomenModel.IsExpanded)
            {                
                var deltaX = blocksCenter.X - blockGroupDomenModel.Position.X;
                var deltaY = blocksCenter.Y - blockGroupDomenModel.Position.Y;
                foreach (var blockDomenModel in blockDomenModels)
                    blockDomenModel.UpdateLayoutPosition(blockDomenModel.Position.X - deltaX + blockGroupDomenModel.Size.Width / 2, blockDomenModel.Position.Y - deltaY + blockGroupDomenModel.Size.Height / 2);
            }
            else
            {
                blockGroupDomenModel.UpdateLayoutPosition(blocksCenter.X - blockGroupDomenModel.Size.Width / 2, blocksCenter.Y - blockGroupDomenModel.Size.Height / 2);
            }
            OnBlockGroupExpandChanged?.Invoke(blockGroupDomenModel.Name, blockGroupDomenModel.IsExpanded);
        }

        public void ToGroupSelectedBlocks(IEnumerable<IBlock> selectedBlocks, string name)
        {
            var selectedIds = selectedBlocks.Select(x => x.Id);
            var center = GetCenter(selectedBlocks.Select(x => x.Position));
            var blockGroupProtocol = new NodeGroup()
            {
                Layout = new Layout { Height = Constants.Numerical.BlockGroupDefaultHeight, Width = Constants.Numerical.BlockGroupDefaultWidth, X = center.X, Y = center.Y },
                Name = name,
                IsExpanded = false,
                Nodes = selectedIds.ToList(),
            };
            var stateCount = selectedBlocks.Count(b => b.Type == NodeType.State);
            var actionCount = selectedBlocks.Count(b => b.Type == NodeType.Action);
            blockGroupDomenCache.Add(new BlockGroup(blockGroupProtocol, stateCount, actionCount, drawingFactory));
            OnBlockGroupCollectionChanged?.Invoke();
        }

        public void UngroupBlockGroup(BlockGroup blockGroupDomenModel)
        {
            if (!blockGroupDomenCache.Remove(blockGroupDomenModel))
                throw new InvalidOperationException(Environment.StackTrace);
            OnBlockGroupCollectionChanged?.Invoke();
        }

        public object GetGraphDrawingModel()
        {
            var domenDataForDrawing = graphBuilder.Build(edgesDomenCache, blockDomenCache, blockGroupDomenCache);
            return drawingFactory.GetGraphDrawingModel(domenDataForDrawing);
        }
        #endregion

        //==============================================================================================

        public void RebuildDomenModels(List<Protocol.Node> protocolBlocks, List<Protocol.Edge> protocolEdge, List<Protocol.NodeGroup> protocolBlockGroup, List<GraphPreset> protocolPresets)
        {
            var domenModels = BuildDomenModels(protocolBlocks, protocolEdge, protocolBlockGroup);
            
            blockDomenCache = domenModels.Item1;
            edgesDomenCache = domenModels.Item2;
            blockGroupDomenCache = domenModels.Item3;
            presetsCache = protocolPresets;
            originDatabaseStateCache = new GraphPreset { NodeState = GetStates() };
        }

        public void UpdateGraphMetadata(string database, string documentId)
        {
            actualDatabase = database;
            actualDocumentId = documentId;
        }

        public void ApplyPresset(PresetDropDown preset)
        {
            GraphPreset targetPreset;
            if (preset is PresetDropDownDefaultGraphState)
                targetPreset = originDatabaseStateCache;
            else if (preset is PresetDropDownRemote)
                targetPreset = presetsCache.Single(x => x.Name == preset.Name);
            else if (preset is PresetDropDownLocal presetDropDownLocal)
                targetPreset = presetDropDownLocal.Preset;
            else
                throw new InvalidOperationException($"Unexpected type{Environment.NewLine}{Environment.StackTrace}");

            ApplyPresset(targetPreset.NodeState, blockDomenCache);
        }

        public void ApplyPresset(GraphPreset preset)
        {
            ApplyPresset(preset.NodeState, blockDomenCache);
        }

        /// <summary>
        /// Apply active blocks and invoke event notification about it
        /// </summary>
        private void ApplyPresset(Dictionary<string, bool> activeBlocks, List<IBlock> blockDomenModels)
        {
            var previosBlockStates = GetStates();
            ApplyActiveBlocks(activeBlocks, blockDomenModels);
            var blockStateDiff = GetDiffStates(previosBlockStates);
            OnStatesUpdateIPC?.Invoke(new BlockStateUpdateArg(blockStateDiff, actualDocumentId, actualDatabase));
        }

        /// <summary>
        /// Apply active blocks WITHOUT event notification about it
        /// </summary>
        public void ApplyActiveBlocksWithoutResponseToExternalClient(Dictionary<string, bool> activeBlocks)
        {
            ApplyActiveBlocks(activeBlocks, blockDomenCache);
        }

        private void ApplyActiveBlocks(Dictionary<string, bool> activeBlocks, List<IBlock> blockDomenModels)
        {
            var stateBlocks = blockDomenModels.OfType<BlockState>();
            foreach (var stateBlock in stateBlocks)
            {
                if (activeBlocks.ContainsKey(stateBlock.Id))
                    stateBlock.SetState(activeBlocks[stateBlock.Id]);
                else
                    stateBlock.SetState(false);
            }
            OnStatesUpdate?.Invoke();
        }

        public void SetStateFromExternalClient(Dictionary<string, bool> activeBlocks)
        {
            foreach (var stateBlockDomen in blockDomenCache.OfType<BlockState>())
            {
                if (activeBlocks.ContainsKey(stateBlockDomen.Id))
                    stateBlockDomen.SetState(activeBlocks[stateBlockDomen.Id]);
            }
            OnStatesUpdate?.Invoke();
        }

        private (List<IBlock>, List<DomenModel.Edge>, List<BlockGroup>) BuildDomenModels(List<Protocol.Node> protocolBlocks, List<Protocol.Edge> protocolEdge, List<Protocol.NodeGroup> protocolBlockGroup)
        {
            #region Convertation from Protocol to Domen
            var blockGroup = protocolBlockGroup.Select(x => 
            {
                var stateCount = protocolBlocks.Count(b => b.Type == NodeType.State && x.Nodes.Contains(b.RefId));
                var actionCount = protocolBlocks.Count(b => b.Type == NodeType.Action && x.Nodes.Contains(b.RefId));
                return new BlockGroup(x, stateCount, actionCount, drawingFactory);
            }).ToList();
            var edges = protocolEdge.Select((x, i) => new DomenModel.Edge(x, i)).ToList();
            var blocks = new List<IBlock>();
            for (var i = 0; i < protocolBlocks.Count; i++)
            {
                var blockProtocolModel = protocolBlocks[i];
                IBlock block = null;
                switch (blockProtocolModel.Type)
                {
                    case NodeType.State:
                        block = new BlockState((Protocol.NodeState)blockProtocolModel, drawingFactory, i);
                        break;
                    case NodeType.Action:
                        block = new BlockAction((Protocol.NodeAction)blockProtocolModel, drawingFactory, i);
                        break;
                    default:
                        throw ExceptionConstructor.UnexpectedEnumValue(blockProtocolModel.Type, Environment.StackTrace);
                }

                blocks.Add(block);
            }
            #endregion

            #region Validation
            var graphStructureValidator = new GraphStructureValidator();
            graphStructureValidator.GraphStructureValidation(blocks, edges);
            #endregion

            #region Set Relations for valid blocks
            var validEdges = edges.Where(x => x.IsValid);
            var validBlocks = blocks.Where(x => x.IsValid);
            var blockRelation = GetBlockRelations(validEdges, validBlocks);
            var stateBlocks = validBlocks.OfType<BlockState>();
            foreach (BlockAction blockAction in validBlocks.OfType<BlockAction>())
            {
                var relationSet = blockRelation.GetBlockRelationSet(blockAction.Id);
                var inputBlocks = stateBlocks.Where(x => relationSet.InputBlocks.Contains(x.Id)).OfType<IBlockState>().ToList();
                var outputBlocks = stateBlocks.Where(x => relationSet.OutputBlocks.Contains(x.Id)).OfType<IBlockState>().ToList();
                blockAction.SetRelations(inputBlocks, outputBlocks);
            }
            #endregion

            return (blocks, edges, blockGroup);
        }

        public BlockRelationDictionary GetBlockRelations(IEnumerable<DomenModel.Edge> edges, IEnumerable<IBlock> blocks)
        {
            var blockRelation = new BlockRelationDictionary();
            foreach (var edge in edges)
            {
                var fromBlocks = blocks.Where(x => x.Id == edge.From).ToList();
                var toBlocks = blocks.Where(x => x.Id == edge.To).ToList();

                if (fromBlocks.Count == 0 || toBlocks.Count == 0)
                {
                    //TODO error notification
                    continue;
                }

                if (fromBlocks.Count > 1 || toBlocks.Count > 1)
                {
                    //TODO error notification
                    continue;
                }

                var fromBlock = fromBlocks.Single();
                var toBlock = toBlocks.Single();

                edge.SetBlockTypes(fromBlock.Type, toBlock.Type);

                blockRelation.AddRelation(fromBlock.Id, toBlock.Id);
                if (edge.Type == EdgeType.Bidirectional)
                    blockRelation.AddRelation(toBlock.Id, fromBlock.Id);
            }

            return blockRelation;
        }

        public Dictionary<string, bool> GetStates()
        {
            return blockDomenCache.Where(x => x.IsValid).OfType<IBlockState>().ToDictionary(x => x.Id, x => x.IsActive);
        }

        public Dictionary<string, bool> GetDiffStates(Dictionary<string, bool> previosBlockStates)
        {
            //TODO алгоритм не совершенен. Непонятно, что будет если такого блока нет или появился новый блок не учтённый
            return GetStates().Where(entry => previosBlockStates[entry.Key] != entry.Value).ToDictionary(entry => entry.Key, entry => entry.Value);            
        }

        public List<IBlock> GetBlocksInfos()
        {
            return blockDomenCache;
        }

        public List<BlockGroup> GetBlockGroupsInfos()
        {
            return blockGroupDomenCache;
        }

        public List<NodeGroup> GetBlockGroupsProtocole()
        {
            return blockGroupDomenCache.Select(x => x.ProtocolModel).ToList();
        }

        public List<DomenModel.Edge> GetConnectionInfos()
        {
            return edgesDomenCache;
        }

        private Point GetCenter(IEnumerable<Point> positions)
        {
            var minX = positions.Min(x => x.X);
            var maxX = positions.Max(x => x.X);
            var minY = positions.Min(x => x.Y);
            var maxY = positions.Max(x => x.Y);
            return new Point(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);
        }

        public void Clean()
        {
            blockGroupDomenCache.Clear();
            edgesDomenCache.Clear();
            blockDomenCache.Clear();
            presetsCache.Clear();
            originDatabaseStateCache = null;
        }
    }
}