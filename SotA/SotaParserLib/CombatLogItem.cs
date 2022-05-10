using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SotaLogParser
{
    public class CombatLogItem : LogItem
    {
        public class CombatResult
        {
            private static Regex regexHit;

            static CombatResult()
            {
                regexHit = new Regex(@"(hits|is\sblocked|is\sparried|is\sout\sof\sreach)\s?,\s+dealing\s+((?<dmg>[0-9,]+)\s+points?\s+of(\s+(?<mod>.+))?|no)+\sdamage((\sdue\sto\s(?<reason>.+)|\spast\sthe\s(?<reason>.+))?\sfrom\s+(?<skill>.+)?|(\sdue\sto\s(?<reason>.+)|\spast\sthe\s(?<reason>.+))?(\s+from\s+(?<skill>.+))?)\.", RegexOptions.Compiled);
            }

            public CombatResult(string result)
            {
                var matchHit = regexHit.Match(result);

                if (matchHit.Success)
                {
                    AttackResult = AttackResults.Hit;

                    Damage = matchHit.Groups["dmg"].Success ? ((int)Decimal.Parse(matchHit.Groups["dmg"].Value, CultureInfo.InvariantCulture)) : 0;

                    Skill = matchHit.Groups["skill"].Success ? matchHit.Groups["skill"].Value : null;

                    if (matchHit.Groups["mod"].Success)
                    {
                        Modifier = matchHit.Groups["mod"].Value;
                    }

                    if (matchHit.Groups["reason"].Success)
                    {
                        Reason = matchHit.Groups["reason"].Value;
                    }
                }
            }

            public CombatResult(AttackResults result, AttackModifiers modifier, string skill, int amount)
            {
                AttackResult = result;
                AttackModifier = modifier;
                Damage = amount;
                Skill = skill;
                AttackModifier = modifier;
                Reason = "reason?";
            }

            public override string ToString()
            {
                switch (AttackResult)
                {
                    case AttackResults.Hit:
                        return Skill is null ? $"Hit (dmg={Damage})" : $"Hit (dmg={Damage}, skill={Skill})";

                    default:
                        return "Unspecified";
                }

            }

            public enum AttackResults
            {
                Unspecified,
                Hit,
                Miss,
                Parried,
                OutOfReach,
                Blocked,
            }

            public enum AttackModifiers
            {
                None,
                Glancing,
                Critical
            }

            public AttackResults AttackResult { get; } = AttackResults.Unspecified;
            public AttackModifiers AttackModifier { get; } = AttackModifiers.None;

            public int Damage { get; }
            public string Skill { get; }
            public string Modifier { get; }
            public string Reason { get; }
        }

        public CombatLogItem(DateTime timestamp, string path, int lineNumber, string line, string restOfLine, string src, string target, string result) : base(timestamp, path, lineNumber, line, restOfLine)
        {
            WhoSource = src;
            WhoTarget = target;
            Result = new CombatResult(result);
        }

        public CombatLogItem(DateTime timestamp, string path, int lineNumber, string line, string restOfLine, string src, string target, CombatResult result) : base(timestamp, path, lineNumber, line, restOfLine)
        {
            WhoSource = src;
            WhoTarget = target;
            Result = result;
        }


        public string WhoSource { get; }
        public string WhoTarget { get; }
        public CombatResult Result { get; }
    }
}
