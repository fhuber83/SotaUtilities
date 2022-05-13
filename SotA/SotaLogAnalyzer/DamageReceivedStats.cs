using System;
using System.Collections.Generic;
using SotaLogParser;

namespace LogAnalyzer
{
    public class DamageReceivedStats
    {
        public DamageReceivedStats(string name)
        {
            Name = name;
        }

        public void Add(CombatLogItem itemBase)
        {
            DamageTotal += itemBase.Result.Damage;
            Items.Add(itemBase);
        }

        public string Name { get; }
        public Int64 DamageTotal { get; private set; } = 0;
        public List<SotaLogParser.CombatLogItem> Items { get; } = new List<CombatLogItem>();
    }
}