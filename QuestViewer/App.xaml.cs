using NLog;
using QuestGraph.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using QuestViewer.ServerForExternalClient;

namespace QuestViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private ApplicationUpdater updater = new ApplicationUpdater();
        private QuestViewerViewModel viewModel;
        private IServerForExternalClient serverForExternalClient;

        protected override void OnStartup(StartupEventArgs e)
        {
            logger.Trace("Starting...");
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            App.Current.DispatcherUnhandledException += UnhandledException;
            TaskScheduler.UnobservedTaskException += UnobservedTaskException;

            FileAndFolderNames.Init();
            var config = Config.Instance;

            //QuestViewerViewModel have to create strongly after FileAndFolderNames initialization
            viewModel = new QuestViewerViewModel(config.DrawingConfig); 
            QuestViewerView questViewerView = new QuestViewerView();
            questViewerView.DataContext = viewModel;
            questViewerView.Show();
            
            viewModel.UpdaterSubscribe(updater);
            updater.RunAppUpdatesObserverRoutine().Track();

            ChronosAPI.Initialization(new Uri(Constants.ChronosWsUri), Constants.ChronosApiKey, "quest_viewer", "client");

            var lastConnection = config.LastConnection;
            if (lastConnection != null)
            {
                var connectionList =
                    SerializationUtils.LoadXml<CdbConnectionSerialization>(FileAndFolderNames.ConnectionsXmlFilePath, (x) => x.Connections = new List<CdbConnection>());
                var connection = connectionList.Connections.FirstOrDefault(x => x.Database == lastConnection.DatabaseName && x.Url == lastConnection.Url);
                if (connection != null)
                    viewModel.AutoConnectByAppRun(connection, lastConnection.DocumentId).Track();
            }

            serverForExternalClient = new HttpServer(viewModel.InterProcessCommunicationModel);
            serverForExternalClient.OnActiveConnectionCountChanged += activeConnectionCount => viewModel.ActiveConnectionCount = activeConnectionCount;
            serverForExternalClient.Start();
            serverForExternalClient.StartListennig().Track();
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            serverForExternalClient.Stop();
            Exception e = (Exception)args.ExceptionObject;
            logger.Error(e, e.GetBaseException().Message);
        }

        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            serverForExternalClient.Stop();
            var ex = e.Exception.GetBaseException();
            logger.Error(ex, $"Unhandled exception: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            e.Handled = true;
        }

        private void UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            serverForExternalClient.Stop();
            var ex = e.Exception.GetBaseException();
            logger.Error(ex, $"Unobserved task exception: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            serverForExternalClient.Stop();
            viewModel.Exit();
            base.OnExit(e);
        }
    }
}
