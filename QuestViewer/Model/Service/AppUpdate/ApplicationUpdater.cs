using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using NLog;
using System.Web;
using System.ComponentModel;

namespace QuestViewer
{
    public class ApplicationUpdater
    {
        /// <summary>
        /// Updated file has been downloaded
        /// </summary>
        public event Action<bool, string> UpdateDownloaded;
        public event Action UpdateDownloading;

        public string RemoteRev { get; private set; }
        public IProgress<AppUpdateProgressModel> DownloadProgress { get; set; }

        private Logger logger = LogManager.GetCurrentClassLogger();
        private bool serverNotFound;
        private HashSet<ApplicationUpdateChannel> updating;
        private string tempFolderPath =>
            Directory.CreateDirectory(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Temp\QuestViewer", "Setup")).FullName;

        public ApplicationUpdater()
        {
            updating = new HashSet<ApplicationUpdateChannel>();
        }

        public void ApplyUpdate()
        {
            var exeFileName = Path.Combine(tempFolderPath, "quest_viewer_setup_" + RemoteRev + ".exe");
            if (string.IsNullOrEmpty(exeFileName))
                return;

            var currentProcessId = Process.GetCurrentProcess().Id;
            foreach (var worker in Process.GetProcessesByName("QuestViewer").Where(x => x.Id != currentProcessId))
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }

            Process.Start(exeFileName, "/SILENT");
            Application.Current.Shutdown();
        }

        public void CheckForUpdates()
        {
            return;

            /*
            if (File.Exists(".ignoreupdate"))
                return;

            CheckForUpdatesAsync()
                .ContinueWith((task) => { });
            */
        }

        private async Task CheckForUpdatesAsync()
        {
            void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                DownloadProgress?.Report(new AppUpdateProgressModel(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive));
            }

            var chanel = Config.Instance.UpdateChannel;
            if (!updating.Add(chanel))
                return;

            string channelUrl = string.Empty;
            try
            {
                channelUrl = Constants.UpdateUrl + chanel.ToString().ToLower();
                WebClient webClient = new WebClient();
                RemoteRev = (await webClient.DownloadStringTaskAsync(channelUrl + "/rev.conf")).Trim();
                var tempPath = tempFolderPath;
                var exeFileName = Path.Combine(tempPath, "quest_viewer_setup_" + RemoteRev + ".exe");

                bool isMatchRevision = RemoteRev == Utils.GetAppRevision();
                if (!isMatchRevision)
                    if (!File.Exists(exeFileName))
                    {
                        Directory
                            .GetFiles(tempPath)
                            .Where(x => x != exeFileName)
                            .ToList()
                            .ForEach(f => File.Delete(f));

                        var tempFileName = Path.Combine(tempPath, "quest_viewer_setup.download");
                        try
                        {
                            webClient.DownloadProgressChanged += WebClientDownloadProgressChanged;
                            UpdateDownloading?.Invoke();
                            await webClient.DownloadFileTaskAsync(channelUrl + @"/quest_viewer_setup.exe", tempFileName);
                            File.Move(tempFileName, exeFileName);
                        }
                        finally
                        {
                            webClient.DownloadProgressChanged -= WebClientDownloadProgressChanged;
                        }
                    }

                UpdateDownloaded?.Invoke(isMatchRevision, RemoteRev);
            }
            catch (WebException ex)
            {
                var webResponse = ex.Response as HttpWebResponse;
                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    if (!serverNotFound)
                    {
                        serverNotFound = true;
                        logger.Trace($"{Texts.ApplycationApdater.ServerNotFound} ({channelUrl})");
                    }
                }
                else
                    logger.Trace($"ApplicationUpdater web exception: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
            catch (IOException ex)
            {
                if (ex.Message.StartsWith("The process cannot access the file"))
                    logger.Trace(ex, ex.Message);
                else
                {
                    logger.Error(ex, ex.GetBaseException().Message);
                    throw;
                }
            }
            finally
            {
                updating.Remove(chanel);
            }
        }

        public async Task RunAppUpdatesObserverRoutine()
        {
            while (true)
            {
                try
                {
                    CheckForUpdates();
                    await Task.Delay(Constants.CheckUpdateDefaultInterval);
                    continue;
                }
                catch (Exception ex)
                {
                    var baseWebEx = ex.GetBaseException() as WebException;
                    if (baseWebEx != null && baseWebEx.Status != WebExceptionStatus.NameResolutionFailure)
                        logger.Error(ex, Texts.ApplycationApdater.CheckApplicationUpdates);
                }
                await Task.Delay(Constants.CheckUpdateDefaultInterval);
            }
        }
    }
}