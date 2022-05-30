using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SotaLogParser;

namespace SotaLogWatcher
{
    /// <summary>
    /// Information about a log file, such as last access time, last size, etc.
    /// </summary>
    internal class FileInfo
    {
        public string Path { get; }
        public DateTime LastProcessed { get; set; }
        public long LastSize { get; set; }

        public SotaLog Log { get; set; }

        public FileInfo(string path)
        {
            Path = path;
            LastProcessed = DateTime.UtcNow;

            var info = new System.IO.FileInfo(path);

            LastSize = info.Length;

            Log = new SotaLog(path);
        }
    }
}
