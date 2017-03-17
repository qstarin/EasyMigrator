using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing;
using EasyMigrator.Tests.Schemas;
using EasyMigrator.Tests.TableTest;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    [TestFixture]
    public class Parse : TableTestBase
    {
        [Test] public void Table1() => Test<Table1>();
        [Test] public void Fk1() => Test<Fk1>();
        [Test] public void ManyToMany1() => Test<ManyToMany1>();


        protected override void Test(ITableTestCase testCase)
        {
            foreach (var data in testCase.Datum)
                AssertEx.AreEqual(data.Model, Parser.Default.ParseTableType(data.Poco).Table, false);
        }
    }
}
