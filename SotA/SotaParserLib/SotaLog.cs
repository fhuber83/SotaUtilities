using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SotaLogParser
{
    public partial class SotaLog
    {
        private static Regex regexTimestamp;
        private static Regex regexTimestamp2;
        private static Regex regexHit;
        private static Regex regexHeal;
        private static Regex regexHealWithoutHealer;
        private static Regex regexChat;
        private static Regex regexLoot;
        private static Regex regexRoll;
        private static Regex regexRollWin;
        private static Regex regexRollStart;
        private static Regex regexHarvest;
        private static Regex regexHarvest2;
        private static Regex regexLevelUp;
        private static Regex regexRhapsodyOfRecovery;
        private static Regex regexDamageAbsorb;
        private static Regex regexAdvLevelUp;
        private static Regex regexProdLevelUp;
        private static Regex regexDamageTaken;

        static SotaLog()
        {
            regexTimestamp = new Regex(@"^\[(?<month>\d+)\/(?<day>\d+)\/(?<year>\d+)\s(?<hour>\d+):(?<minute>\d+):(?<second>\d+)\s(?<AmPm>[AP]M)\]\s(?<rest>.*)$", RegexOptions.Compiled);
            regexTimestamp2 = new Regex(@"^\[\d{2}:\d{2}\]\s+(?<rest>.*)$", RegexOptions.Compiled);

            regexHit = new Regex(@"^(?<attacker>[A-Za-z\(\)0-9'\<\> ]+)(\[PVP\])?\sattacks\s(?<target>[A-Za-z'\(\)0-9\<\> ]+) and (?<result>.*)$", RegexOptions.Compiled);
            regexHeal = new Regex(@"^(?<patient>[A-Za-z\(\)0-9'\<\> ]+)(\[PVP\])?\sis\shealed\sfor\s(?<amount>[0-9,]+)\spoints?\sof\shealth(?<crit>\s\(critical\)\s|\s)by\s(?<healer>[A-Za-z\(\)0-9'\<\> ]+)(\[PVP\])?.$", RegexOptions.Compiled);
            regexHealWithoutHealer = new Regex(@"^(?<patient>[A-Za-z\(\)0-9'\<\> ]+)(\[PVP\])?\sis\shealed\sfor\s(?<amount>[0-9,]+)\spoints?\sof\shealth(?<crit>\s\(critical\)\.|\.)$", RegexOptions.Compiled);
            regexChat = new Regex(@"^(?<name>[A-Za-z<>' ]+)(\[PVP\])?\s\(To\s(?<chat>[A-Za-z ]+)(\[PVP\])?\):\s(?<msg>.+)$", RegexOptions.Compiled);
            regexLoot = new Regex(@"^(?<name>[A-Za-z<>' ]+)\slooted\san\sitem\s\((?<item>.+)\)\.$", RegexOptions.Compiled);
            regexRoll = new Regex(@"^(?<name>[A-Za-z<>' ]+)\srolled\s(?<roll>\d+)$", RegexOptions.Compiled);
            regexRollWin = new Regex(@"^(?<name>[A-Za-z<>' ]+)\sWON\swith\s(?<roll>\d+)$", RegexOptions.Compiled);
            regexRollStart = new Regex(@"^Roll\sresults\sfor\s(?<item>[A-Za-z\-\(\)\+,:' ]+)\s+\(\s+Value:\s+(?<value>\d+)\)$", RegexOptions.Compiled);
            regexHarvest = new Regex(@"^(?<name>[A-Za-z<>' ]+)\sharvested\san\sitem\s\((?<item>.+)\)\.", RegexOptions.Compiled);
            regexHarvest2 = new Regex(@"^Item\s\((?<item>.*)\)\sadded\sto\sinventory\.$", RegexOptions.Compiled);
            regexLevelUp = new Regex(@"^(?<name>[A-Za-z<>' ]+)'s\sskill\s\((?<skill>[A-Za-z' -]+)\)\shas\sincreased\sto\slevel\s(?<level>\d+)!$", RegexOptions.Compiled);
            regexRhapsodyOfRecovery = new Regex(@"^(?<player>[A-Za-z0-9<>'\<\> ]+)(\[PVP\])?\sadds\s(?<amount>\d+)\spoints of focus by (?<source>[A-Za-z<>'\<\> ]+(\[PVP\])?'s )?Rhapsody of Recovery.$", RegexOptions.Compiled);
            regexDamageAbsorb = new Regex(@"^([A-Za-z<>' ]+)\ absorbs (\d+) points of damage through ([-0-9]+) focus.$", RegexOptions.Compiled);
            regexAdvLevelUp = new Regex(@"^(?<name>[A-Za-z<>' ]+)'s Adventurer level has increased to (?<level>\d+)!$", RegexOptions.Compiled);
            regexProdLevelUp = new Regex(@"^(?<name>[A-Za-z<>' ]+)'s Producer level has increased to (?<level>\d+)!$", RegexOptions.Compiled);
            
            // TODO this Regex probably needs some work!
            regexDamageTaken = new Regex(@"^(?<who>[A-Za-z\(\)0-9'\<\> ]+)(\[PVP\])?\stakes\s(?<amount>\d+)\spoints\sof\sdamage(\sfrom\s(?<skill>.*))?.$", RegexOptions.Compiled);
        }

        public SotaLog()
        {
        }

        public SotaLog(string path)
        {
            Path = path;
        }

        private string Path;
        private int LineNumber = 1;
        private RollStateMachine RollSTM = new RollStateMachine();

        /// <summary>
        /// Tries to parse a line of text from a SotA log file
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public LogItem ParseLine(string line)
        {
            LogItem parsedItem = null;

            var matchTime = regexTimestamp.Match(line);

            if (matchTime.Success)
            {
                var month = int.Parse(matchTime.Groups["month"].Value);
                var day = int.Parse(matchTime.Groups["day"].Value);
                var year = int.Parse(matchTime.Groups["year"].Value);

                var hour = int.Parse(matchTime.Groups["hour"].Value);
                var minute = int.Parse(matchTime.Groups["minute"].Value);
                var second = int.Parse(matchTime.Groups["second"].Value);
                var isPM = matchTime.Groups["AmPm"].Value.ToUpper().Equals("PM");

                AmPmConverter.Convert12To24(isPM ? AmPmConverter.AmPm.PM : AmPmConverter.AmPm.AM, hour, out hour);

                var timestamp = new DateTime(year, month, day, hour, minute, second);

                var restOfLine = matchTime.Groups["rest"].Value;

                // Strip optional second timestamp
                var matchTimestamp2 = regexTimestamp2.Match(restOfLine);

                if (matchTimestamp2.Success)
                {
                    restOfLine = matchTimestamp2.Groups["rest"].Value;
                }

                var matchHit = regexHit.Match(restOfLine);

                if (matchHit.Success)
                {
                    try
                    {
                        var src = matchHit.Groups["attacker"].Value;
                        var trgt = matchHit.Groups["target"].Value;

                        var item = new CombatLogItem(timestamp, Path, LineNumber, line, restOfLine, src, trgt, matchHit.Groups["result"].Value);

                        Items.Add(item);
                        CombatItems.Add(item);

                        parsedItem = item;
                    }
                    catch
                    {
                        ErrorLines.Add(line);
                    }
                }
                else
                {
                    var matchHeal = regexHeal.Match(restOfLine);

                    if (matchHeal.Success)
                    {
                        var healer = matchHeal.Groups["healer"].Value;
                        var healee = matchHeal.Groups["patient"].Value;
                        int amount = (int)Decimal.Parse(matchHeal.Groups["amount"].Value, CultureInfo.InvariantCulture);
                        var crit = matchHeal.Groups["crit"].Value.Trim().Equals("(critical)");

                        var item = new HealItem(timestamp, Path, LineNumber, line, restOfLine, healer, healee, amount, crit);

                        HealItems.Add(item);
                        Items.Add(item);
                        
                        parsedItem = item;
                    }

                    else
                    {
                        var matchHealWithoutHealer = regexHealWithoutHealer.Match(
                            restOfLine);

                        if (matchHealWithoutHealer.Success)
                        {
                            var healee = matchHealWithoutHealer.Groups["patient"].Value;
                            var amount = (int)Decimal.Parse(matchHealWithoutHealer.Groups["amount"].Value,
                                CultureInfo.InvariantCulture);

                            var item = new HealItem(timestamp, Path, LineNumber, line, restOfLine, "(Null)", healee,
                                amount, false);

                            HealItems.Add(item);
                            Items.Add(item);

                            parsedItem = item;
                        }

                        else
                        {
                            var matchChat = regexChat.Match(restOfLine);

                            // Chat message
                            if (matchChat.Success)
                            {
                                var name = matchChat.Groups["name"].Value;
                                var chatName = matchChat.Groups["chat"].Value;
                                var message = matchChat.Groups["msg"].Value;

                                var item = new ChatItem(timestamp, Path, LineNumber, line, restOfLine, name,
                                    chatName, message);

                                ChatItems.Add(item);
                                Items.Add(item);

                                parsedItem = item;
                            }
                            else
                            {
                                // Direct loot
                                var matchLoot = regexLoot.Match(restOfLine);

                                if (matchLoot.Success)
                                {
                                    var looterName = matchLoot.Groups["name"].Value;
                                    var itemName = matchLoot.Groups["item"].Value;

                                    var item = new LootItem(timestamp, Path, LineNumber, line, restOfLine,
                                        itemName, looterName);

                                    LootItems.Add(item);
                                    Items.Add(item);

                                    parsedItem = item;
                                }
                                else
                                {
                                    // Roll for loot
                                    var matchRoll = regexRoll.Match(restOfLine);

                                    if (matchRoll.Success)
                                    {
                                        try
                                        {
                                            var looterName = matchRoll.Groups["name"].Value;
                                            var rollResult = int.Parse(matchRoll.Groups["roll"].Value);

                                            RollSTM.Roll(looterName, rollResult, false, line, restOfLine);
                                        }
                                        catch
                                        {
                                            RollSTM.Reset();

                                            var item = new LogItem(timestamp, Path, LineNumber, line,
                                                restOfLine);

                                            Items.Add(item);
                                            MiscItems.Add(item);

                                            parsedItem = item;
                                        }
                                    }
                                    else
                                    {
                                        var matchRollWin = regexRollWin.Match(restOfLine);

                                        // Loot win
                                        if (matchRollWin.Success)
                                        {
                                            try
                                            {
                                                var looterName = matchRollWin.Groups["name"].Value;
                                                var rollResult = int.Parse(matchRollWin.Groups["roll"].Value);

                                                RollSTM.Roll(looterName, rollResult, true, line, restOfLine);
                                            }
                                            catch
                                            {
                                                RollSTM.Reset();

                                                var item = new LogItem(timestamp, Path, LineNumber, line,
                                                    restOfLine);

                                                Items.Add(item);
                                                MiscItems.Add(item);

                                                parsedItem = item;
                                            }
                                        }
                                        else
                                        {
                                            var matchRollStart = regexRollStart.Match(restOfLine);

                                            // Rolling for an item begins
                                            if (matchRollStart.Success)
                                            {
                                                try
                                                {
                                                    var itemName = matchRollStart.Groups["item"].Value;
                                                    var itemValue =
                                                        int.Parse(matchRollStart.Groups["value"].Value);

                                                    if (RollSTM.HasValidItem)
                                                    {
                                                        var item = RollSTM.GetItem();

                                                        LootItems.Add(item);
                                                        Items.Add(item);

                                                        parsedItem = item;
                                                    }

                                                    RollSTM.Begin(timestamp, Path, LineNumber, itemName,
                                                        itemValue, line, restOfLine);
                                                }
                                                catch
                                                {
                                                    RollSTM.Reset();

                                                    var item = new LogItem(timestamp, Path, LineNumber, line,
                                                        restOfLine);

                                                    Items.Add(item);
                                                    MiscItems.Add(item);

                                                    parsedItem = item;
                                                }
                                            }
                                            else
                                            {
                                                var matchHarvest = regexHarvest.Match(restOfLine);

                                                if (matchHarvest.Success)
                                                {
                                                    var harvesterName = matchHarvest.Groups["name"].Value;
                                                    var itemName = matchHarvest.Groups["item"].Value;

                                                    var item = new LootItem(timestamp, Path, LineNumber, line,
                                                        restOfLine, itemName, harvesterName);

                                                    LootItems.Add(item);
                                                    Items.Add(item);

                                                    parsedItem = item;
                                                }
                                                else
                                                {
                                                    var matchHarvest2 = regexHarvest2.Match(restOfLine);

                                                    if (matchHarvest2.Success)
                                                    {
                                                        var itemName = matchHarvest2.Groups["item"].Value;

                                                        var item = new LootItem(timestamp, Path, LineNumber,
                                                            line, restOfLine, itemName, null);

                                                        LootItems.Add(item);
                                                        Items.Add(item);

                                                        parsedItem = item;
                                                    }
                                                    else
                                                    {
                                                        var matchLevelUp = regexLevelUp.Match(restOfLine);

                                                        if (matchLevelUp.Success)
                                                        {
                                                            var name = matchLevelUp.Groups["name"].Value;
                                                            var skill = matchLevelUp.Groups["skill"].Value;
                                                            var level = int.Parse(matchLevelUp.Groups["level"]
                                                                .Value);

                                                            var item = new LevelUpItem(timestamp, Path,
                                                                LineNumber, line, restOfLine, name, level,
                                                                skill);

                                                            LevelUpItems.Add(item);
                                                            Items.Add(item);

                                                            parsedItem = item;
                                                        }

                                                        else
                                                        {
                                                            var matchRhapsodyOfRecovery =
                                                                regexRhapsodyOfRecovery.Match(restOfLine);

                                                            if (matchRhapsodyOfRecovery.Success)
                                                            {
                                                                try
                                                                {
                                                                    var name = matchRhapsodyOfRecovery.Groups["player"]
                                                                        .Value;
                                                                    var amount = int.Parse(
                                                                        matchRhapsodyOfRecovery
                                                                            .Groups["amount"].Value);

                                                                    string caster = null;

                                                                    if (matchRhapsodyOfRecovery.Groups["source"].Success)
                                                                    {
                                                                        caster = matchRhapsodyOfRecovery
                                                                            .Groups["source"]
                                                                            .Value;
                                                                    }

                                                                    var item = new RhapsodyOfRecoveryItem(
                                                                        timestamp,
                                                                        Path, LineNumber, line, restOfLine,
                                                                        name,
                                                                        caster, amount);

                                                                    RhapsodyOfRecoveryItems.Add(item);
                                                                    Items.Add(item);

                                                                    parsedItem = item;
                                                                }
                                                                catch
                                                                {
                                                                    var item = new LogItem(timestamp, Path,
                                                                        LineNumber,
                                                                        line, restOfLine);

                                                                    Items.Add(item);
                                                                    parsedItem = item;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var matchAbsorb =
                                                                    regexDamageAbsorb.Match(restOfLine);

                                                                if (matchAbsorb.Success)
                                                                {
                                                                    try
                                                                    {
                                                                        var who = matchAbsorb.Groups[1].Value;
                                                                        var amountDmg =
                                                                            int.Parse(matchAbsorb.Groups[2]
                                                                                .Value);
                                                                        var amountFocus =
                                                                            int.Parse(matchAbsorb.Groups[3]
                                                                                .Value);

                                                                        var item = new DamageAbsorbItem(
                                                                            timestamp,
                                                                            Path, LineNumber, line, restOfLine,
                                                                            who,
                                                                            amountDmg, amountFocus);

                                                                        DamageAbsorbItems.Add(item);
                                                                        Items.Add(item);
                                                                        
                                                                        parsedItem = item;
                                                                    }
                                                                    catch
                                                                    {
                                                                        var item = new LogItem(timestamp, Path,
                                                                            LineNumber, line, restOfLine);

                                                                        Items.Add(item);
                                                                        parsedItem = item;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var matchAdvLevelUp =
                                                                        regexAdvLevelUp.Match(restOfLine);

                                                                    if (matchAdvLevelUp.Success)
                                                                    {
                                                                        var charName =
                                                                            matchAdvLevelUp.Groups["name"]
                                                                                .Value;

                                                                        var advLevel =
                                                                            int.Parse(matchAdvLevelUp
                                                                                .Groups["level"].Value);

                                                                        var item = new LevelUpItem(timestamp,
                                                                            Path, LineNumber, line, restOfLine,
                                                                            charName, advLevel, "Adventurer Level");

                                                                        Items.Add(item);
                                                                        LevelUpItems.Add(item);
                                                                        parsedItem = item;
                                                                    }
                                                                    else
                                                                    {
                                                                        var matchProdLevelUp =
                                                                            regexProdLevelUp.Match(restOfLine);

                                                                        if (matchProdLevelUp.Success)
                                                                        {
                                                                            var charName =
                                                                                matchProdLevelUp.Groups["name"]
                                                                                    .Value;

                                                                            var prodLevel =
                                                                                int.Parse(matchProdLevelUp
                                                                                    .Groups["level"].Value);

                                                                            var item = new LevelUpItem(timestamp,
                                                                                Path, LineNumber, line, restOfLine,
                                                                                charName, prodLevel, "Producer Level");

                                                                            Items.Add(item);
                                                                            LevelUpItems.Add(item);
                                                                            parsedItem = item;
                                                                        }

                                                                        else
                                                                        {
                                                                            var matchDamageTaken =
                                                                                regexDamageTaken.Match(
                                                                                    restOfLine);

                                                                            if (matchDamageTaken.Success)
                                                                            {
                                                                                var who = matchDamageTaken
                                                                                    .Groups["who"].Value;

                                                                                var amount =
                                                                                    int.Parse(matchDamageTaken
                                                                                        .Groups["amount"]
                                                                                        .Value);

                                                                                string skill = matchDamageTaken.Groups["skill"].Success ? matchDamageTaken.Groups["skill"].Value : null;

                                                                                var combatItem =
                                                                                    new CombatLogItem.
                                                                                        CombatResult(
                                                                                            CombatLogItem
                                                                                                .CombatResult
                                                                                                .AttackResults
                                                                                                .Hit,
                                                                                            CombatLogItem
                                                                                                .CombatResult
                                                                                                .AttackModifiers
                                                                                                .None, skill,
                                                                                            amount);

                                                                                var item = new CombatLogItem(
                                                                                    timestamp, Path, LineNumber,
                                                                                    line, restOfLine, "(Null)", who,
                                                                                    combatItem);

                                                                                CombatItems.Add(item);
                                                                                Items.Add(item);
                                                                                parsedItem = item;
                                                                            }

                                                                            else
                                                                            {
                                                                                var item = new LogItem(
                                                                                    timestamp,
                                                                                    Path,
                                                                                    LineNumber,
                                                                                    line, restOfLine);

                                                                                Items.Add(item);
                                                                                MiscItems.Add(item);
                                                                                parsedItem = item;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ErrorLines.Add(line);
            }

            return parsedItem;
        }

        public Task LoadAsync(string path, Action<double> progress, CancellationToken cancel)
        {
            return Task.Run(() =>
            {
                var lines = System.IO.File.ReadAllLines(path);

                var rollSTM = new RollStateMachine();

                int lineNumber = 0;

                foreach (var line in lines)
                {
                    lineNumber += 1;

                    if (cancel.IsCancellationRequested)
                    {
                        break;
                    }

                    ParseLine(line);

                    // Report progress
                    if(!(progress is null))
                    {
                        if((lineNumber % 1000) == 0)
                        {
                            progress((lineNumber * 100.0) / lines.Length);
                        }
                    }
                }

                if (rollSTM.HasValidItem)
                {
                    var item = rollSTM.GetItem();
                    LootItems.Add(item);
                    Items.Add(item);
                }
            });
        }

        public List<string> ErrorLines { get; } = new List<string>();
        public List<LogItem> Items { get; } = new List<LogItem>();
        public List<CombatLogItem> CombatItems { get; } = new List<CombatLogItem>();
        public List<HealItem> HealItems { get; } = new List<HealItem>();
        public List<ChatItem> ChatItems { get; } = new List<ChatItem>();
        public List<LogItem> MiscItems { get; } = new List<LogItem>();
        public List<LootItem> LootItems { get; } = new List<LootItem>();
        public List<LevelUpItem> LevelUpItems { get; } = new List<LevelUpItem>();
        public List<RhapsodyOfRecoveryItem> RhapsodyOfRecoveryItems { get; } = new List<RhapsodyOfRecoveryItem>();
        public List<DamageAbsorbItem> DamageAbsorbItems { get; } = new List<DamageAbsorbItem>();

        public SotaLog Filter(DateTime minTime, DateTime maxTime, List<string> playerNames, List<string> chatNames)
        {
            var filtered = new SotaLog();

            CombatItems.Where(x => x.Timestamp >= minTime && x.Timestamp <= maxTime && playerNames.Contains(x.WhoSource))
                .Select(x => x)
                .ToList()
                .ForEach(x =>
            {
                filtered.CombatItems.Add(x);
                filtered.Items.Add(x);
            });

            HealItems.Where(x => x.Timestamp >= minTime && x.Timestamp <= maxTime && playerNames.Contains(x.HealerName))
                .Select(x => x)
                .ToList()
                .ForEach(x =>
            {
                filtered.HealItems.Add(x);
                filtered.Items.Add(x);
            });

            ChatItems.Where(x => x.Timestamp >= minTime && x.Timestamp <= maxTime && chatNames.Contains(x.ChatName))
                .Select(x => x)
                .ToList()
                .ForEach(x =>
            {
                filtered.ChatItems.Add(x);
                filtered.Items.Add(x);
            });

            MiscItems.Where(x => x.Timestamp >= minTime && x.Timestamp <= maxTime).Select(x => x).ToList().ForEach(x =>
            {
                filtered.MiscItems.Add(x);
                filtered.Items.Add(x);
            });

            LootItems.Where(x => x.Timestamp >= minTime && x.Timestamp <= maxTime && playerNames.Contains(x.LooterName))
                .Select(x => x)
                .ToList()
                .ForEach(x =>
            {
                filtered.LootItems.Add(x);
                filtered.Items.Add(x);
            });

            return filtered;
        }
    }
}
