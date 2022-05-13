namespace SotaParserTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void ConstructsAndIsEmpty()
        {
            // Check that SotaLogParser can be constructed
            var log = new SotaLogParser.SotaLog();

            // Freshly created log must be empty
            Assert.That(log.Items.Count, Is.EqualTo(0));
        }


        [TestCase(@"[1/16/2022 9:14:30 PM] [21:14] Frost Giant tried to attack Snowball <Cowboy Bill> but Snowball <Cowboy Bill> dodges.", @"Frost Giant tried to attack Snowball <Cowboy Bill> but Snowball <Cowboy Bill> dodges.")]
        public void TimestampIsStrippedFromLine(string line, string restOfLine)
        {
            var log = new SotaLogParser.SotaLog();

            var item = log.ParseLine(line);

            Assert.That(item.Line, Is.EqualTo(line));
            Assert.That(item.RestOfLine, Is.EqualTo(restOfLine));
        }


        [TestCase(@"[4/20/2022 6:27:21 PM] [18:27] Sheamous Spearshaker attacks Brign, Lord of Water and is blocked , dealing 57 points of damage past the block from Thrust.", @"2022-04-20 18:27:21")]
        public void TimestampStringIsFormattedCorrectly(string line, string timestampString)
        {
            var log = new SotaLogParser.SotaLog();

            var item = log.ParseLine(line);

            Assert.That(item.TimestampString, Is.EqualTo(timestampString));
        }


        [TestCase(@"[5/12/2022 12:28:43 PM] [12:28] Red Devils <Gamechanger> attacks Brign, Lord of Water and hits, dealing 224 points of damage.", 5, 12, 2022, 12, 28, 43)]
        [TestCase(@"[1/11/2022 11:39:37 PM] [23:39] Christ Pallid is now online.", 1, 11, 2022, 23, 39, 37)]
        public void CanParseTimestamp(string line, int month, int day, int year, int hour, int minute, int second)
        {
            var log = new SotaLogParser.SotaLog();

            var item = log.ParseLine(line);

            Assert.That(item.Timestamp.Month, Is.EqualTo(month));
            Assert.That(item.Timestamp.Day, Is.EqualTo(day));
            Assert.That(item.Timestamp.Year, Is.EqualTo(year));
            Assert.That(item.Timestamp.Hour, Is.EqualTo(hour));
            Assert.That(item.Timestamp.Minute, Is.EqualTo(minute));
            Assert.That(item.Timestamp.Second, Is.EqualTo(second));
        }


        [TestCase(@"[5/12/2022 12:28:43 PM] [12:28] Red Devils <Gamechanger> attacks Brign, Lord of Water and hits, dealing 224 points of damage.", "Red Devils <Gamechanger>", "Brign, Lord of Water", 224)]
        [TestCase(@"[5/12/2022 11:51:30 AM] [11:51] Brign, Lord of Water attacks Red Devils <Gamechanger> and hits, dealing 5 points of damage from Ice Field.", "Brign, Lord of Water", "Red Devils <Gamechanger>", 5)]
        [TestCase(@"[5/12/2022 9:08:33 PM] [21:08] Böser Wind <Rielle Peddler> attacks Kobold Mage and hits, dealing 105 points of damage.", "Böser Wind <Rielle Peddler>", "Kobold Mage", 105)]
        [TestCase(@"[5/12/2022 11:25:34 AM] [11:25] Sheamous Spearshaker takes 17 points of damage from Spike Damage.", "(Null)", "Sheamous Spearshaker", 17)]
        public void CanParseCombatItem(string line, string attacker, string target, int damage)
        {
            var log = new SotaLogParser.SotaLog();

            var item = log.ParseLine(line);

            Assert.That(item, Is.InstanceOf<SotaLogParser.CombatLogItem>());

            if(item is SotaLogParser.CombatLogItem combatItem)
            {
                Assert.That(combatItem.WhoSource, Is.EqualTo(attacker));
                Assert.That(combatItem.WhoTarget, Is.EqualTo(target));
                Assert.That(combatItem.Result.Damage, Is.EqualTo(damage));
            }
        }


        [TestCase(@"[5/12/2022 11:47:50 AM] [11:47] Red Devils <Gamechanger> is healed for 26 points of health by Gamechanger.", "Gamechanger", "Red Devils <Gamechanger>", 26)]
        [TestCase(@"[5/12/2022 11:48:19 AM] [11:48] Gamechanger is healed for 26 points of health.", "(Null)", "Gamechanger", 26)]
        public void CanParseHealItem(string line, string healer, string patient, int amount)
        {
            var log = new SotaLogParser.SotaLog();

            var item = log.ParseLine(line);

            Assert.That(item, Is.InstanceOf<SotaLogParser.HealItem>());

            if(item is SotaLogParser.HealItem healItem)
            {
                Assert.That(healItem.HealerName, Is.EqualTo(healer));
                Assert.That(healItem.PatientName, Is.EqualTo(patient));
                Assert.That(healItem.HealAmount, Is.EqualTo(amount));
            }
        }


        [TestCase(@"[5/12/2022 11:47:32 AM] [11:47] Gamechanger adds 11 points of focus by Rhapsody of Recovery.", "Gamechanger", "(Null)", 11)]
        [TestCase(@"[5/12/2022 11:47:34 AM] [11:47] Sheamous Spearshaker adds 11 points of focus by Gamechanger's Rhapsody of Recovery.", "Sheamous Spearshaker", "Gamechanger", 11)]
        public void CanParseRhapsodyOfRecoveryItem(string line, string whoGotFocus, string whoseRhapsody, int amount)
        {
            var log = new SotaLogParser.SotaLog();

            var item = log.ParseLine(line);

            Assert.That(item, Is.InstanceOf<SotaLogParser.RhapsodyOfRecoveryItem>());

            if(item is SotaLogParser.RhapsodyOfRecoveryItem rhapsodyItem)
            {
                Assert.That(rhapsodyItem.PlayerName, Is.EqualTo(whoGotFocus));
                Assert.That(rhapsodyItem.RhapsodyCasterName, Is.EqualTo(whoseRhapsody));
                Assert.That(rhapsodyItem.FocusRecovered, Is.EqualTo(amount));
            }
        }


        [TestCase(@"[4/30/2022 4:27:48 AM] [04:27] Drakkar Khan's skill (Mesmerizing Melody) has increased to level 45!", "Drakkar Khan", "Mesmerizing Melody", 45)]
        [TestCase(@"[5/11/2022 9:09:39 AM] [09:09] Sheamous Spearshaker's Producer level has increased to 121!", "Sheamous Spearshaker", "Producer Level", 121)]
        [TestCase(@"[5/12/2022 3:58:46 PM] [15:58] Sheamous Spearshaker's Adventurer level has increased to 148!", "Sheamous Spearshaker", "Adventurer Level", 148)]
        public void CanParseLevelUpItem(string line, string player, string skill, int newLevel)
        {
            var log = new SotaLogParser.SotaLog();

            var item = log.ParseLine(line);

            Assert.That(item, Is.Not.Null);
            Assert.That(item, Is.InstanceOf<SotaLogParser.LevelUpItem>());

            if(item is SotaLogParser.LevelUpItem levelUpItem)
            {
                Assert.That(levelUpItem.Name, Is.EqualTo(player));
                Assert.That(levelUpItem.Skill, Is.EqualTo(skill));
                Assert.That(levelUpItem.Level, Is.EqualTo(newLevel));
            }
        }
    }
}