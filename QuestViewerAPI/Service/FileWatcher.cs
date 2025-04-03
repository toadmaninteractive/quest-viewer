using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuestGraph.Core
{
    public class FileWatcher : IDisposable
    {
        public event Action OnFileChanged;

        private FileSystemWatcher fileSystemWatcher;

        public FileWatcher(string directory, string fileName)
        {
            fileSystemWatcher = new FileSystemWatcher(directory);
            fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                                          | NotifyFilters.CreationTime
                                          | NotifyFilters.DirectoryName
                                          | NotifyFilters.FileName
                                          | NotifyFilters.LastWrite
                                          | NotifyFilters.Size;
            fileSystemWatcher.IncludeSubdirectories = false;
            fileSystemWatcher.Filter = fileName;

            fileSystemWatcher.Changed += FileSystemWatcherChangedHandler;
            fileSystemWatcher.Created += FileSystemWatcherChangedHandler;
            fileSystemWatcher.Renamed += FileSystemWatcherChangedHandler;
            fileSystemWatcher.Deleted += FileSystemWatcherChangedHandler;
        }

        public void EnableRaisingEvents(bool enableRaisingEvents)
        {
            fileSystemWatcher.EnableRaisingEvents = enableRaisingEvents;
        }

        private void FileSystemWatcherChangedHandler(object sender, FileSystemEventArgs e)
        {
            OnFileChanged?.Invoke();
        }

        public void Dispose()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Changed -= FileSystemWatcherChangedHandler;
            fileSystemWatcher.Created -= FileSystemWatcherChangedHandler;
            fileSystemWatcher.Renamed -= FileSystemWatcherChangedHandler;
            fileSystemWatcher.Deleted -= FileSystemWatcherChangedHandler;
        }
    }
}