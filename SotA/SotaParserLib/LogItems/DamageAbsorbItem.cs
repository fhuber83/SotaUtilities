using System;
using System.Collections.Generic;
using System.Text;

namespace SotaLogParser
{
    public class DamageAbsorbItem : LogItemBase
    {
        public DamageAbsorbItem(DateTime timestamp, string? path, int lineNumber, string line, string restOfLine, string playerName, int damageAbsorbed, int focusLost) : base(timestamp, path, lineNumber, line, restOfLine)
        {
            PlayerName = playerName;
            DamageAbsorbed = damageAbsorbed;
            FocusLost = focusLost;
        }

        public string PlayerName { get; set; }
        public int DamageAbsorbed { get; set; }
        public int FocusLost { get; set; }
    }
}
