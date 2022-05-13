using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SotaParserTests
{
    [TestFixture]
    public class RollStateMachineTests
    {
        [Test]
        public void Test1()
        {
            var stm = new SotaLogParser.RollStateMachine();

            Assert.That(stm.HasValidItem, Is.False);

            /*
                [5/13/2022 11:53:42 PM] [11:53] Roll results for Supply Bundle (Orange) ( Value: 1000)
                [5/13/2022 11:53:42 PM] [11:53] Ythak DeThartas rolled 65
                [5/13/2022 11:53:42 PM] [11:53] Sheamous Spearshaker rolled 57
                [5/13/2022 11:53:42 PM] [11:53] Fina Mondklinge WON with 79
             */

            var timestamp = new DateTime(year: 2022, month: 5, day: 13, hour: 11, minute: 53, second: 42);

            stm.Begin(time: timestamp,
                "test.log",
                lineNumber: 42,
                "Supply Bundle (Orange)",
                value: 1_000,
                line: @"[5/13/2022 11:53:42 PM] [11:53] Roll results for Supply Bundle (Orange) ( Value: 1000)",
                lineRest: @"Roll results for Supply Bundle (Orange) ( Value: 1000)");

            Assert.That(stm.HasValidItem, Is.True);
            Assert.That(stm.HasWinner, Is.False);

            stm.Roll("Ythak DeThartas",
                65,
                false,
                @"[5/13/2022 11:53:42 PM] [11:53] Ythak DeThartas rolled 65",
                @"Ythak DeThartas rolled 65");

            Assert.That(stm.HasWinner, Is.False);

            stm.Roll("Sheamous Spearshaker",
                57,
                false,
                @"[5/13/2022 11:53:42 PM] [11:53] Sheamous Spearshaker rolled 57",
                @"Sheamous Spearshaker rolled 57");

            Assert.That(stm.HasWinner, Is.False);

            stm.Roll("Fina Mondklinge",
                79,
                true,
                @"[5/13/2022 11:53:42 PM] [11:53] Fina Mondklinge WON with 79",
                @"Fina Mondklinge WON with 79");

            Assert.That(stm.HasWinner, Is.True);

            var item = stm.GetItem();

            Assert.That(item, Is.Not.Null);

            Assert.That(item, Is.InstanceOf<SotaLogParser.LootRollItem>());

            Assert.That(item.ItemName, Is.EqualTo("Supply Bundle (Orange)"));
            Assert.That(item.LooterName, Is.EqualTo("Fina Mondklinge"));
            Assert.That(item.ItemValue, Is.EqualTo(1000));
            
            Assert.That(item.Rolls, Contains.Key("Sheamous Spearshaker"));
            Assert.That(item.Rolls, Contains.Key("Ythak DeThartas"));
            Assert.That(item.Rolls, Contains.Key("Fina Mondklinge"));

            Assert.That(item.Rolls["Sheamous Spearshaker"], Is.EqualTo(57));
            Assert.That(item.Rolls["Ythak DeThartas"], Is.EqualTo(65));
            Assert.That(item.Rolls["Fina Mondklinge"], Is.EqualTo(79));
        }

        [Test]
        public void Test2()
        {
            var stm = new SotaLogParser.RollStateMachine();

            Assert.That(() => { stm.Roll("RollerName", 42, true, "", ""); }, Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void Test3()
        {
            var stm = new SotaLogParser.RollStateMachine();

            Assert.That(() => { var item = stm.GetItem(); }, Throws.InstanceOf<InvalidOperationException>());
        }
    }
}
