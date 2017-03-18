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
        [Test] public void AutoInc_Custom_Byte() => Test<AutoInc_Custom_Byte>();
        [Test] public void AutoInc_Custom_Int16() => Test<AutoInc_Custom_Int16>();
        [Test] public void AutoInc_Custom_Int32() => Test<AutoInc_Custom_Int32>();
        [Test] public void AutoInc_Custom_Int64() => Test<AutoInc_Custom_Int64>();
        [Test] public void Fk_ByType_Guid() => Test<Fk_ByType_Guid>();
        [Test] public void Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>();
        [Test] public void Mtm_BookAuthors() => Test<Mtm_BookAuthors>();
        [Test] public void Table1() => Test<Table1>();

        protected override void Test(ITableTestCase testCase)
        {
            foreach (var data in testCase.Datum)
                AssertEx.AreEqual(data.Model, Parser.Default.ParseTableType(data.Poco).Table, false, false);
        }
    }
}
