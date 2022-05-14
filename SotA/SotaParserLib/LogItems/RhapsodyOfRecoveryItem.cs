using System;
using System.Collections.Generic;
using System.Text;

namespace SotaLogParser
{
    public class RhapsodyOfRecoveryItem : LogItemBase
    {
        public RhapsodyOfRecoveryItem(DateTime timestamp, string? path, int lineNumber, string line, string restOfLine, string playerName, string casterName, int amount) : base(timestamp, path, lineNumber, line, restOfLine)
        {
            PlayerName = playerName;
            RhapsodyCasterName = casterName;
            FocusRecovered = amount;
        }

        public string PlayerName { get; set; }
        public string RhapsodyCasterName { get; set; }
        public int FocusRecovered { get; set; }
    }
}
