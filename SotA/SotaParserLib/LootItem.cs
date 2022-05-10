using System;
using System.Collections.Generic;
using System.Text;

namespace SotaLogParser
{
    public class LootItem : LogItem
    {
        public LootItem(DateTime time, string path, int lineNumber, string line, string restOfLine, string itemName, string looterName, int itemValue = 0) : base(time, path, lineNumber, line, restOfLine)
        {
            ItemName = itemName;
            LooterName = looterName;
            ItemValue = itemValue;
        }

        public string ItemName { get; }
        public string LooterName { get; }
        public int ItemValue { get; } = 0;
    }

    public class LootRollItem : LootItem
    {
        public LootRollItem(DateTime time, string path, int lineNumber, string line, string restOfLine, string itemName, string looterName, int itemValue) : base(time, path, lineNumber, line, restOfLine, itemName, looterName, itemValue)
        {
        }

        public void AddRoll(string name, int roll)
        {
            Rolls.Add(name, roll);
        }
        
        public Dictionary<string, int> Rolls { get; } = new Dictionary<string, int>();
    }
}
