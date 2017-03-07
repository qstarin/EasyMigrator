using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    [TestFixture]
    public class Parse : TableTestBase
    {
        [Test] public void Table1() { Test<Data.Table1>(); }


        override protected void Test(ITableTestCase testCase)
        {
            foreach (var data in testCase.Datum)
                AssertEx.AreEqual(data.Model, Parser.Default.ParseTableType(data.Poco));
        }
    }
}
