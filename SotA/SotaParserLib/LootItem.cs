using System;
using System.Collections.Generic;
using System.Text;

namespace SotaLogParser
{
    public class LootItem : LogItem
    {
        public LootItem(DateTime time, string path, int lineNumber, string line, string restOfLine, string itemName, string looterName) : base(time, path, lineNumber, line, restOfLine)
        {
            ItemName = itemName;
            LooterName = looterName;
        }

        public string ItemName { get; }
        public string LooterName { get; }
    }

    public class LootRollItem : LootItem
    {
        public LootRollItem(DateTime time, string path, int lineNumber, string line, string restOfLine, string itemName, string looterName, int itemValue) : base(time, path, lineNumber, line, restOfLine, itemName, looterName)
        {
            ItemValue = itemValue;
        }

        public void AddRoll(string name, int roll)
        {
            Rolls.Add(name, roll);
        }

        public int ItemValue { get; }

        public Dictionary<string, int> Rolls { get; } = new Dictionary<string, int>();
    }
}
