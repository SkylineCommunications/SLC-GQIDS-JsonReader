namespace GQI___JSON_creator_1
{
    using System;
    using System.Collections.Concurrent;
    using GQIDS___JSON_reader_1;
    using Skyline.DataMiner.Net.Messages;

    internal static class ReaderCache
    {
        private static bool _isCacheValid;
        private static object _lock = new object();
        private static ConcurrentDictionary<string, Reader> _cache = new ConcurrentDictionary<string, Reader>(StringComparer.OrdinalIgnoreCase);

        static ReaderCache()
        {
            Watcher.CacheInvalidated += Watcher_CacheInvalidated;
        }

        internal static Reader GetReader(string path)
        {
            if (_isCacheValid && _cache.TryGetValue(path, out Reader reader))
                return reader;

            lock (_lock)
            {
                reader = new Reader(path);
                _cache[path] = reader;
                _isCacheValid = true;
                return reader;
            }
        }


        private static void Watcher_CacheInvalidated(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _isCacheValid = false;
                _cache.Clear();
            }
        }
    }
}
