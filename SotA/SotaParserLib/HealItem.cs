using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SotaLogParser
{
    public class HealItem : LogItem
    {
        public HealItem(DateTime timestamp, string path, int lineNumber, string line, string restOfLine, string nameHealer, string namePatient, int amount, bool crit) : base(timestamp, path, lineNumber, line, restOfLine)
        {
            HealerName = nameHealer;
            PatientName = namePatient;
            HealAmount = amount;
            Critical = crit;
        }

        public string HealerName { get; }
        public string PatientName { get; }
        public int HealAmount { get;  }
        public bool Critical { get; }
    }
}
