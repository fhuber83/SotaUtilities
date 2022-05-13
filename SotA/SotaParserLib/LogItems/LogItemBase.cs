using System;

namespace SotaLogParser
{
    /// <summary>
    /// Base class for all "Items" that can result from parsing a SotA log file. This is usually
    /// extended by more specific subclasses; however this class is also instanced for "misc" items
    /// that cannot be parsed into something more specific (e.g. the output of Lua extensions)
    /// </summary>
    public class LogItemBase
    {
        public DateTime Timestamp { get; }
        public string TimestampString { get; }
        public string Line { get; }
        public string RestOfLine { get; }
        public int LineNumber { get; set;  }
        public string FileName { get; set;  }

        public LogItemBase(DateTime timestamp, string fileName, int lineNumber, string line, string restOfLine)
        {
            Timestamp = timestamp;
            TimestampString = $"{timestamp.Year:D4}-{timestamp.Month:D2}-{timestamp.Day:D2} {timestamp.Hour:D2}:{timestamp.Minute:D2}:{timestamp.Second:D2}";
            FileName = fileName;

            LineNumber = lineNumber;

            Line = line;
            RestOfLine = restOfLine;
        }
    }
}
