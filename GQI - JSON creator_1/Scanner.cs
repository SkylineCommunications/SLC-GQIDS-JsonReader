namespace GQIDS___JSON_reader_1
{
	using System;
	using System.IO;
	using System.Linq;
	using GQI___JSON_creator_1;
	using Skyline.DataMiner.Net.Messages;

    internal static class Scanner
    {
        private static bool _isCacheValid;
        private static object _lock = new object();
        private static string[] _cache = null;

        static Scanner()
        {
            Watcher.CacheInvalidated += Watcher_CacheInvalidated;
        }

        internal static string[] Scan()
        {
            if (_isCacheValid)
                return _cache;

            lock (_lock)
            {
                var results = Directory.GetFiles(JSONReaderDataSource.DIRECTORY, $"*.json", SearchOption.AllDirectories);
                _cache = results?.Select(x => x.Substring(JSONReaderDataSource.DIRECTORY.Length)).ToArray() ?? new string[0];
                _isCacheValid = true;
                return _cache;
            }
        }

        private static void Watcher_CacheInvalidated(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _isCacheValid = false;
            }
        }
    }
}
