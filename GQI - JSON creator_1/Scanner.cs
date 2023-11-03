using System;
using System.IO;
using System.Linq;

namespace GQIDS___JSON_reader_1
{
    internal static class Scanner
    {
        internal const string DIRECTORY = @"C:\Skyline DataMiner\Documents\GQI data sources\";
        private const int CACHE_DURATION = 15;
        private static DateTime lastScan = DateTime.MinValue;
        private static string[] lastResultCache = null;

        internal static string[] Scan()
        {
            if (lastScan > DateTime.UtcNow - TimeSpan.FromSeconds(CACHE_DURATION))
                return lastResultCache;

            var results = Directory.GetFiles(DIRECTORY, $"*.json", SearchOption.AllDirectories);
            lastResultCache = results?.Select(x => x.Substring(DIRECTORY.Length)).ToArray() ?? new string[0];
            lastScan = DateTime.UtcNow;
            return lastResultCache;
        }
    }
}
