using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QuestGraph.Core;
using QuestGraph.Core.DomenModel;
using QuestGraph.Core.Protocol;
using NLog;
using QuestViewer.Service;
using Json;
using System.Reactive.Concurrency;
using System.Threading;
using System.Reactive.Linq;
using System.Diagnostics;

namespace QuestViewer
{
    /// <summary>
    /// Main QuestViewer window viewModel
    /// </summary>
    public class QuestViewerViewModel : NotifyPropertyChanged
    {
        #region events for update QuestViewer window without bindings 
        public event Action OnGraphClean;
        public event Action<bool> OnGraphRedraw;
        public event Action OnConnectionRouting;
        #endregion

        public event Action<string> OnBlockGroupSelected;

        public ICommand OpenCouchDBConnectionManagerCommand { get; }
        public ICommand<string> UpdateSelectedGraphIdCommand { get; }
        public ICommand<PresetDropDown> UpdateCurrentPresetCommand { get; }

        public ICommand LocalStateManagerCommand { get; }
        public ICommand SaveStateAsLocalPresetCommand { get; }
        public ICommand AppSettingsCommand { get; }
        public ICommand UpdateApplyCommand { get; private set; }
        public ICommand ConnectToDBCommand { get; private set; }
        public ICommand ShowConnectDbManagerCommand { get; private set; }
        public ICommand AddConnectDbCommand { get; private set; }
        public ICommand CloseConnectDbCommand { get; private set; }
        public ICommand BlockGroupCollapseCommand { get; private set; }
        public ICommand ConnectionRoutingCommand { get; private set; }
        public ICommand ApplyRemoteUpdateToGraphCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }
        public ICommand ShowGridCommand { get; private set; }
        public ICommand OpenLogsCommand { get; private set; }

        #region Application update  
        public IProgress<AppUpdateProgressModel> AppUpdateDownloadProgress { get; }
        public bool HasAppUpdate
        {
            get { return hasAppUpdate; }
            set { SetField(ref hasAppUpdate, value); }
        }
        public bool IsAppUpdateDownloading
        {
            get { return isAppUpdateDownloading; }
            set { SetField(ref isAppUpdateDownloading, value); }
        }
        public string AppUpdateDownloadingText
        {
            get { return appUpdateDownloadingText; }
            set { SetField(ref appUpdateDownloadingText, value); }
        }
        public long TotalBytesToReceive
        {
            get { return totalBytesToReceive; }
            set { SetField(ref totalBytesToReceive, value); }
        }
        public long BytesToReceive
        {
            get { return bytesToReceive; }
            set { SetField(ref bytesToReceive, value); }
        }
        public ApplicationUpdater Updater;
        #endregion

        /// <summary>
        /// Get new object for drawing in UI
        /// </summary>
        public object NextGraphDrawingModel 
        {
            get
            {
                graphDrawingModel = quest.GetGraphDrawingModel();
                return graphDrawingModel;
            }
        }
        public List<IBlock> BlockInfos => quest.GetBlocksInfos();
        public IGraphInteractive InteractiveModel => quest;
        public IInterProcessCommunication InterProcessCommunicationModel => quest;
        public EdgesRoutingMode RoutingMode => quest.RoutingMode;
        public List<BlockGroup> BlockGroupInfos => quest.GetBlockGroupsInfos();
        public BlockGroup SelectedBlockGroup
        {
            get { return selectedBlockGroup; }
            set 
            { 
                SetField(ref selectedBlockGroup, value);
                if (IsLoaded && value != null)
                    OnBlockGroupSelected?.Invoke(value.Name);
            }
        }        

        public List<CdbConnection> DbConnections 
        { 
            get { return dbConnections; }
            set { SetField(ref dbConnections, value); }
        }
        public string CurrentConnectionName
        {
            get { return currentConnectionName; }
            set 
            { 
                SetField(ref currentConnectionName, value);
                UpdateTitle();
            }
        }
        public List<string> AvailableGraphIds
        {
            get { return availableGraphIds; }
            set 
            { 
                SetField(ref availableGraphIds, value);
                IsLoaded = AvailableGraphIds?.Any() ?? false;
            }
        }
        public string SelectedGraphId
        {
            get { return selectedGraphId; }
            set 
            {
                IsBusy = true;
                SetField(ref selectedGraphId, value);
                logger.Debug($"Switching to {value} Graph...");
                if (IsLoaded)
                {
                    var config = Config.Instance;
                    config.LastConnection = new LastConnection(currentConnection.Database, currentConnection.Url, value);
                    config.Save();
                    SetupAndRedrawQuestGraph(value).Track();
                }
            }
        }
        public List<PresetDropDown> Presets
        {
            get { return presets; }
            set { SetField(ref presets, value); }
        }
        public PresetDropDown CurrentPreset
        {
            get { return currentPreset; }
            set 
            { 
                SetField(ref currentPreset, value);
                if (value == null) //Calls from Presets property change
                    return;
                quest.ApplyPresset(value);
            }
        }
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { SetField(ref isLoaded, value); }
        }
        public string SelectedGraphComment
        {
            get { return selectedGraphComment; }
            set { SetField(ref selectedGraphComment, value); }
        }
        public bool HasGraphRemoteUpdate
        {
            get { return hasGraphRemoteUpdate; }
            set 
            { 
                SetField(ref hasGraphRemoteUpdate, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public string Title
        {
            get { return title; }
            set { SetField(ref title, value); }
        }
        public bool IsGridVisible
        {
            get { return isGridVisible; }
            set { SetField(ref isGridVisible, value); }
        }
        public bool IsConnectionRouting
        {
            get => isConnectionRouting;
            set => SetField(ref isConnectionRouting, value);
        }
        public int ActiveConnectionCount 
        { 
            get => activeConnectionCount; 
            set => SetField(ref activeConnectionCount, value); 
        }
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetField(ref isBusy, value); CommandManager.InvalidateRequerySuggested(); }
        }

        private bool hasAppUpdate;
        private bool isAppUpdateDownloading;
        private string appUpdateDownloadingText;
        private long bytesToReceive;
        private long totalBytesToReceive;
        private bool hasUpdateCancelation;
        private string lastRemoteRev = string.Empty;

        private List<CdbConnection> dbConnections;
        private string currentConnectionName;
        private List<string> availableGraphIds = new List<string>();//Be sure to initialize for binding (or will NullReferenceException in XAML binding)
        private string selectedGraphId;
        private string selectedGraphComment;
        private List<PresetDropDown> presets = new List<PresetDropDown>();//Be sure to initialize for binding (or will NullReferenceException in XAML binding)
        private PresetDropDown currentPreset;
        private bool isLoaded;
        private bool hasGraphRemoteUpdate;
        private string title;
        private bool isGridVisible;
        private bool isConnectionRouting;
        private int activeConnectionCount;
        private bool isBusy;
        private BackendChange graphRemoteChanges;
        private BlockGroup selectedBlockGroup;

        private Quest quest;
        private CdbServer cdbServer;
        private CdbConnection currentConnection;
        private object graphDrawingModel;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private string userSavesFolderName;
        private bool isQuestGraphSetUp;
        IDisposable cdbServerListener;

        public QuestViewerViewModel(DrawingConfiguration drawingConfiguration)
        {
            quest = new Quest(drawingConfiguration);

            OpenCouchDBConnectionManagerCommand = new AsyncCommand(OpenCouchDBConnectionManagerHandler);
            UpdateSelectedGraphIdCommand = new RelayCommand<string>((x) => SelectedGraphId = x, (x) => IsLoaded);
            UpdateCurrentPresetCommand = new RelayCommand<PresetDropDown>((x) => CurrentPreset = x, (x) => IsLoaded);

            LocalStateManagerCommand = new RelayCommand(LocalStateManagerHandler, () => IsLoaded);
            SaveStateAsLocalPresetCommand = new RelayCommand(SaveStateAsLocalPresetHandler, () => IsLoaded);
            AppSettingsCommand = new RelayCommand(AppSettingsHandler);
            BlockGroupCollapseCommand = new RelayCommand(BlockGroupExpandCollapseHandler, () => IsLoaded);
            ConnectionRoutingCommand = new RelayCommand(ConnectionRoutingHandler, () => IsLoaded);
            ApplyRemoteUpdateToGraphCommand = new RelayCommand(ApplyRemoteUpdateToGraphHandler, () => IsLoaded && HasGraphRemoteUpdate);

            ConnectToDBCommand = new AsyncCommand<CdbConnection>(Reconnect, () => DbConnections.Any());
            ShowConnectDbManagerCommand = new AsyncCommand(OpenCouchDBConnectionManagerHandler);
            AddConnectDbCommand = new AsyncCommand(AddConnectHandler);
            CloseConnectDbCommand = new RelayCommand(CloseConnectDbHandler);

            AboutCommand = new RelayCommand(() => 
            {
                var aboutView = new AboutDialogView();
                aboutView.Owner = Application.Current.Windows.OfType<QuestViewerView>().Single();
                aboutView.ShowDialog();
            });
            ShowGridCommand = new RelayCommand(() => IsGridVisible = !IsGridVisible, () => IsLoaded);
            OpenLogsCommand = new RelayCommand(OpenLogsFolderHandler);

            IsConnectionRouting = RoutingMode != EdgesRoutingMode.Non;
            AppUpdateDownloadProgress = new Progress<AppUpdateProgressModel>(ApplicationUpdateDownloadProgress);
            quest.OnBlockGroupExpandChanged += (name, isExpanded) => BlockGroupSerialize();
            UpdateDbConnectionList();
            UpdateTitle();
        }

        private async Task OpenCouchDBConnectionManagerHandler()
        {
            var windowOwner = Application.Current.Windows.OfType<QuestViewerView>().Single();
            var connectionManagerViewModel = new ConnectionManagerViewModel(windowOwner, currentConnection);
            var connectionManager = new ConnectionManager();
            connectionManager.Owner = windowOwner;
            connectionManager.DataContext = connectionManagerViewModel;
            connectionManagerViewModel.LoadConnectios();
            if (connectionManager.ShowDialog() ?? false)
                await Reconnect(connectionManagerViewModel.SelectedConnecton);

            UpdateDbConnectionList();
        }

        private async Task AddConnectHandler()
        {
            var windowOwner = Application.Current.Windows.OfType<QuestViewerView>().Single();
            var connectionManager = new ConnectionManagerViewModel(windowOwner, currentConnection);
            connectionManager.LoadConnectios();
            if (connectionManager.AddConnectionHandler(out CdbConnection newDbConnaction))
            {
                UpdateDbConnectionList();
                if (IsLoaded)
                    CloseConnectDbHandler();
                await Connect(newDbConnaction);
                if (AvailableGraphIds.Any())
                    SelectedGraphId = AvailableGraphIds.First();
            }
        }

        private void CloseConnectDbHandler()
        {
            if (cdbServerListener != null)
            {
                cdbServerListener.Dispose();
                cdbServerListener = null;
            }

            CurrentPreset = null;
            SelectedGraphId = null;

            Presets.Clear();
            AvailableGraphIds.Clear();

            CurrentConnectionName = null;

            RaisePropertyChanged(nameof(Presets));
            RaisePropertyChanged(nameof(AvailableGraphIds));
            UpdateTitle();

            quest.Clean();

            var config = Config.Instance;
            config.LastConnection = null;
            config.Save();

            OnGraphClean?.Invoke();
        }

        private async Task Connect(CdbConnection connection)
        {
            IsBusy = true;
            try
            {
                Config.UserNameFromCurrentConnection = connection.Username;
                ScyllaConnector.LoadInstance().SendSessionStart().Track();
                logger.Info("Connecting...");
            }
            catch (Exception ex)
            {
                logger.Trace(ex);
            }

            var newGraphs = new List<string>();
            currentConnection = connection;
            CurrentConnectionName = connection.Title;
            cdbServer = new CdbServer(new Uri(currentConnection.Url), currentConnection.Username, currentConnection.Password);

            var documentNamesResponce = await cdbServer.GetQuestGraphDocumentsAsync(currentConnection.Database);
            if (documentNamesResponce.IsSuccessStatusCode)
            {
                newGraphs = documentNamesResponce.Value.ToList();
            }
            else
            {
                try
                {
                    var documents = await cdbServer.GetAllDocumentsAsync(currentConnection.Database);
                    foreach (var graphCard in documents.AsObject["rows"].AsArray)
                    {
                        var graphCardObject = graphCard.AsObject;
                        var graphCardObjectId = graphCardObject["id"].AsString;
                        if (graphCardObjectId == "schema" || graphCardObjectId == "diagram_schema")
                            continue;

                        newGraphs.Add(graphCardObjectId);
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                { 
                    if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MessageBox.Show(string.Format(Texts.CodeErrors.DataBaseIsNotFound, currentConnection.Url, currentConnection.Database), Texts.OperationFailedCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        logger.Warn(ex, ex.GetBaseException().Message);
                    }
                    else
                        logger.Error(ex, $"{ex.GetBaseException().Message}{Environment.NewLine}{Environment.StackTrace}");
                    IsBusy = false;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{ex.GetBaseException().Message}{Environment.NewLine}{Environment.StackTrace}");
                    IsBusy = false;
                }                
            }

            AvailableGraphIds = newGraphs;
            var wpfAppScheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);

            var sinceSeq = await cdbServer.GetLastSinceSeq(currentConnection.Database);
            cdbServerListener = cdbServer.ListenForChanges(currentConnection.Database, sinceSeq)
                .ObserveOn(wpfAppScheduler)
                .Subscribe(DbServerUpdateNotificationCallback);

            logger.Info("Connection has been loaded");
        }

        public async Task AutoConnectByAppRun(CdbConnection connection, string documentId)
        {
            await Connect(connection);
            if (AvailableGraphIds.Contains(documentId))
                SelectedGraphId = documentId;
        }

        private async Task Reconnect(CdbConnection connection)
        {
            if (IsLoaded)
                CloseConnectDbHandler();
            await Connect(connection);
            if (AvailableGraphIds.Any())
                SelectedGraphId = AvailableGraphIds.First();
        }

        private void UpdateDbConnectionList()
        {
            var connectionSerializationObject = SerializationUtils.LoadXml<CdbConnectionSerialization>(FileAndFolderNames.ConnectionsXmlFilePath, (x) => x.Connections = new List<CdbConnection>());
            DbConnections = connectionSerializationObject.Connections;
        }

        /// <summary>
        /// Rebuild domen model only
        /// </summary>
        private async Task SetupAndRedrawQuestGraph(string graphRefId)
        {
            var document = await cdbServer.GetDocumentAsync(currentConnection.Database, graphRefId);
            SetupAndRedrawQuestGraph(graphRefId, document);
        }

        private void SetupAndRedrawQuestGraph(string graphRefId, ImmutableJson document, bool isAutoFit = true)
        {
            logger.Debug($"Setup and redrawing to {graphRefId} Graph...");
            HasGraphRemoteUpdate = false;
            if (document.IsObject && document.AsObject.TryGetValue("category", out ImmutableJson category) && category.AsString == "quest_graph")
            {
                SelectedGraphComment = string.Empty;
                var selectedGraphProtocolModel = new CardQuestGraphJsonSerializer().Deserialize(document);

                userSavesFolderName = Path.Combine(FileAndFolderNames.UserLocalSavesFolder, HashProvider.GetHashString(currentConnection.Url, currentConnection.Database, graphRefId));
                var protocolBlockGroup = new BlockGroupFromFileLoader().Load(userSavesFolderName);
                quest.RebuildDomenModels(selectedGraphProtocolModel.Nodes, selectedGraphProtocolModel.Edges, protocolBlockGroup, selectedGraphProtocolModel.Presets);
                quest.UpdateGraphMetadata(currentConnection.Database, graphRefId);
                RaisePropertyChanged(nameof(BlockGroupInfos));

                var remotePresets = selectedGraphProtocolModel.Presets.Select(x => new PresetDropDownRemote(x.Name, x.Name)).OfType<PresetDropDown>().ToList();
                var graphLocalPresets = new GraphStateFromFileLoader().Load(userSavesFolderName).Select(x => new PresetDropDownLocal(x)).OfType<PresetDropDown>().ToList();
                var databaseStatePreset = new PresetDropDownDefaultGraphState("OriginState", Texts.View.QuestViewer.Original);

                remotePresets.AddRange(graphLocalPresets);
                remotePresets.Insert(0, databaseStatePreset);
                Presets = remotePresets;
                currentPreset = databaseStatePreset;
                isQuestGraphSetUp = true;
                RaisePropertyChanged(nameof(CurrentPreset));
                logger.Debug($"Setup and redrawing {graphRefId} Graph has compleated");
                OnGraphRedraw?.Invoke(isAutoFit);
            }
            else
            {
                SelectedGraphComment = Texts.View.QuestViewer.IsNotQuestGraph;
            }
        }

        private void BlockGroupExpandCollapseHandler()
        {
            var selectedGroupDomenModel = quest.DrawingModelService.GetSelectedBlockGroupDomenModels(graphDrawingModel).FirstOrDefault();
            if (selectedGroupDomenModel != null)
                quest.InteractWithBlockGroup(selectedGroupDomenModel);
        }

        private void LocalStateManagerHandler()
        {
            var stateManagerView = new LocalStateManagerView();
            var stateManagerViewModel = new LocalStateManagerViewModel(userSavesFolderName, quest.GetStates(), stateManagerView);
            stateManagerView.DataContext = stateManagerViewModel;
            stateManagerView.Owner = Application.Current.Windows.OfType<QuestViewerView>().Single();
            stateManagerView.ShowDialog();
            UpdateLocalPresetDropDownList(stateManagerViewModel.LocalStates.Select(x => new PresetDropDownLocal(x)));
        }

        private void SaveStateAsLocalPresetHandler()
        {
            var stateManagerViewModel = new LocalStateManagerViewModel(userSavesFolderName, quest.GetStates(), Application.Current.Windows.OfType<QuestViewerView>().Single());
            stateManagerViewModel.SaveStateHandler();
            UpdateLocalPresetDropDownList(stateManagerViewModel.LocalStates.Select(x => new PresetDropDownLocal(x)));
        }

        private void UpdateLocalPresetDropDownList(IEnumerable<PresetDropDownLocal> localPresets) 
        {
            foreach (var localPreset in Presets.Where(x => x is PresetDropDownLocal).ToList())
                Presets.Remove(localPreset);
            Presets.AddRange(localPresets);

            if (!Presets.Select(x => x.Name).Contains(CurrentPreset.Name))
                CurrentPreset = Presets.Single(x => x is PresetDropDownDefaultGraphState);
        }

        private void ConnectionRoutingHandler()
        {
            var edgesRoutingMode = RoutingMode == EdgesRoutingMode.Non ? EdgesRoutingMode.AStarRouting : EdgesRoutingMode.Non;
            IsConnectionRouting = edgesRoutingMode != EdgesRoutingMode.Non;
            quest.UpdateDrawingConfiguration(edgesRoutingMode);
            Config.Instance.UpdateEdgesRoutingMode(edgesRoutingMode);
            OnConnectionRouting.Invoke();
        }

        private void AppSettingsHandler()
        {
            AppSettings view = new AppSettings();
            view.Owner = Application.Current.Windows.OfType<QuestViewerView>().Single();
            if (view.ShowDialog() ?? false)
            {
                if (view.IsUpdateChannelHasChanged)
                    Updater.CheckForUpdates();
            }
        }

        #region Application update
        public void UpdaterSubscribe(ApplicationUpdater updater)
        {
            if (Updater != null)
            {
                Updater.UpdateDownloaded -= UpdateNotification;
                Updater.UpdateDownloading -= UpdateDownloading;
            }

            Updater = updater;
            Updater.UpdateDownloaded += UpdateNotification;
            Updater.UpdateDownloading += UpdateDownloading;
            Updater.DownloadProgress = AppUpdateDownloadProgress;

            UpdateApplyCommand = new RelayCommand(Updater.ApplyUpdate);
            RaisePropertyChanged(nameof(UpdateApplyCommand));
        }

        public void UpdateNotification(bool isMatchRevision, string currentRemoteRev)
        {
            IsAppUpdateDownloading = false;

            if (currentRemoteRev != lastRemoteRev)
                hasUpdateCancelation = false;

            if (!hasUpdateCancelation)
                HasAppUpdate = !isMatchRevision;

            lastRemoteRev = currentRemoteRev;
        }

        public void UpdateDownloading()
        {
            IsAppUpdateDownloading = true;
        }

        private void ApplicationUpdateDownloadProgress(AppUpdateProgressModel downloadProgress)
        {
            BytesToReceive = downloadProgress.BytesReceived;
            TotalBytesToReceive = downloadProgress.TotalBytesToReceive;
            AppUpdateDownloadingText = $"Downloading update: {(double)downloadProgress.BytesReceived * 100 / downloadProgress.TotalBytesToReceive:F2} %";
        }
        #endregion

        private void BlockGroupSerialize()
        {
            if (!isQuestGraphSetUp)
                return;

            BlockGroupToFileSaver blockGroupToFileSaver = new BlockGroupToFileSaver();
            blockGroupToFileSaver.Save(userSavesFolderName, quest.GetBlockGroupsProtocole());
        }

        #region graph remote update
        private void DbServerUpdateNotificationCallback(BackendChange change)
        {
            if (!change.Deleted && change.Id == selectedGraphId)
            {
                graphRemoteChanges = change;
                HasGraphRemoteUpdate = true;
            }
        }

        private void ApplyRemoteUpdateToGraphHandler()
        {
            SetupAndRedrawQuestGraph(SelectedGraphId, graphRemoteChanges.Doc, false);
        }
        #endregion

        public void Exit()
        {
            BlockGroupSerialize();
        }

        private void UpdateTitle()
        {
            var mainPart =$"{Texts.View.QuestViewer.Title} {Utils.GetAppVersionRevosionFormat()}";
            Title = string.IsNullOrWhiteSpace(currentConnectionName) ? mainPart : $"{mainPart} - {currentConnectionName}";
        }

        private void OpenLogsFolderHandler()
        {
            var logsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuestViewer");
            if (Directory.Exists(logsFolderPath))
            {
                try
                {
                    Process.Start("explorer.exe", logsFolderPath);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{ex.GetBaseException().Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }
    }
}