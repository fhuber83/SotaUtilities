using System;

namespace SotaLogParser
{
    public class LogItem
    {
        public DateTime Timestamp { get; }
        public string TimestampString { get; }
        public string Line { get; }
        public string RestOfLine { get; }
        public int LineNumber { get; }
        public string FileName { get; }

        public LogItem(DateTime timestamp, string fileName, int lineNumber, string line, string restOfLine)
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
