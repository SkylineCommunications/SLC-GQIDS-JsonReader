namespace GQI___JSON_creator_1
{
    using System;
    using System.IO;
    using GQIDS___JSON_reader_1;

    internal static class Watcher
    {
        private static FileSystemWatcher _watcher;

        static Watcher()
        {
            _watcher = new FileSystemWatcher(JSONReaderDataSource.DIRECTORY);
            _watcher.IncludeSubdirectories = true;

            _watcher.Created += Watcher_Created;
            _watcher.Changed += Watcher_Changed;
            _watcher.Deleted += Watcher_Deleted;
            _watcher.Renamed += Watcher_Renamed;

            _watcher.EnableRaisingEvents = true;
        }

        internal static event EventHandler CacheInvalidated;

        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            CacheInvalidated?.Invoke(sender, e);
        }

        private static void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            CacheInvalidated?.Invoke(sender, e);
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            CacheInvalidated?.Invoke(sender, e);
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            CacheInvalidated?.Invoke(sender, e);
        }
    }
}
