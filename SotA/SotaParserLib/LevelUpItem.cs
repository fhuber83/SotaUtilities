using System;
using System.Collections.Generic;
using System.Text;

namespace SotaLogParser
{
    public class LevelUpItem : LogItem
    {
        public string Name { get; }
        public int Level { get; }
        public string Skill { get; }

        public LevelUpItem(DateTime timestamp, string fileName, int lineNumber, string line, string restOfLine, string name, int level, string skill) : base(timestamp, fileName, lineNumber, line, restOfLine)
        {
            Name = name;
            Level = level;
            Skill = skill;
        }
    }
}
