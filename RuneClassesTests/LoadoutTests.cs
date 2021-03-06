﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Timers;
using System.Diagnostics;

namespace RuneOptim.Tests
{
    [TestClass()]
    public class LoadoutTests
    {

        [TestMethod()]
        public void LockTest()
        {
            Loadout load = new Loadout();
            Rune[] runes = new Rune[6];
            load.AddRune(runes[0] = TestData.Rune1());
            load.AddRune(runes[1] = TestData.Rune2());
            load.AddRune(runes[2] = TestData.Rune3());
            load.AddRune(runes[3] = TestData.Rune4());
            load.AddRune(runes[4] = TestData.Rune5());
            load.AddRune(runes[5] = TestData.Rune6());

            load.Lock();
            foreach (var r in runes)
            {
                Assert.IsTrue(r.Locked);
            }
        }

        [TestMethod()]
        public void AddRuneTest()
        {
            Loadout load = new Loadout();
            load.AddRune(TestData.Rune1());
            load.AddRune(TestData.Rune2());
            load.AddRune(TestData.Rune3());
            load.AddRune(TestData.Rune4());
            load.AddRune(TestData.Rune5());
            load.AddRune(TestData.Rune6());

            Assert.AreEqual(6, load.RuneCount);
        }

        [TestMethod()]
        public void RemoveRuneTest()
        {
            Loadout load = new Loadout();
            load.AddRune(TestData.Rune1());
            load.AddRune(TestData.Rune2());
            load.AddRune(TestData.Rune3());
            load.AddRune(TestData.Rune4());
            load.AddRune(TestData.Rune5());
            load.AddRune(TestData.Rune6());

            load.RemoveRune(1);
            load.RemoveRune(2);
            load.RemoveRune(3);
            load.RemoveRune(4);
            load.RemoveRune(5);
            load.RemoveRune(6);

            Assert.AreEqual(0, load.RuneCount);
        }

        [TestMethod()]
        public void CheckSetsTest()
        {
            Loadout load = new Loadout();
            load.AddRune(TestData.Rune1());
            load.AddRune(TestData.Rune2());
            load.AddRune(TestData.Rune3());
            load.AddRune(TestData.Rune4());
            load.AddRune(TestData.Rune5());
            load.AddRune(TestData.Rune6());

            Assert.IsTrue(load.Sets.Contains(RuneSet.Energy));
            Assert.IsTrue(load.Sets.Contains(RuneSet.Swift));
        }

        [TestMethod()]
        public void SetStatTest()
        {
            Loadout load = new Loadout();
            load.AddRune(TestData.Rune1());
            load.AddRune(TestData.Rune2());
            load.AddRune(TestData.Rune3());
            load.AddRune(TestData.Rune4());
            load.AddRune(TestData.Rune5());
            load.AddRune(TestData.Rune6());

            Assert.AreEqual(15, load.SetStat(Attr.HealthPercent));
            Assert.AreEqual(25, load.SetStat(Attr.SpeedPercent));
        }

        [TestMethod()]
        public void GetStatsTest()
        {
            Loadout load = new Loadout();
            load.AddRune(TestData.Rune1());
            load.AddRune(TestData.Rune2());
            load.AddRune(TestData.Rune3());
            load.AddRune(TestData.Rune4());
            load.AddRune(TestData.Rune5());
            load.AddRune(TestData.Rune6());

            var statsComp = TestData.statsFull();
            var statRhs = load.GetStats(TestData.statsBase());

            Assert.AreEqual(statsComp.Health, statRhs.Health);
            Assert.AreEqual(statsComp.Attack, statRhs.Attack);
            Assert.AreEqual(statsComp.Defense, statRhs.Defense);
            Assert.AreEqual(statsComp.Speed, statRhs.Speed);
            Assert.AreEqual(statsComp.Accuracy, statRhs.Accuracy);
            Assert.AreEqual(statsComp.Resistance, statRhs.Resistance);
            Assert.AreEqual(statsComp.CritRate, statRhs.CritRate);
            Assert.AreEqual(statsComp.CritDamage, statRhs.CritDamage);

            Assert.AreEqual(statsComp.EffectiveHP, statRhs.EffectiveHP);
            Assert.AreEqual(statsComp.EffectiveHPDefenseBreak, statRhs.EffectiveHPDefenseBreak);
            Assert.AreEqual(statsComp.MaxDamage, statRhs.MaxDamage);
            Assert.AreEqual(statsComp.AverageDamage, statRhs.AverageDamage);
            Assert.AreEqual(statsComp.DamagePerSpeed, statRhs.DamagePerSpeed);
        }
    }
}