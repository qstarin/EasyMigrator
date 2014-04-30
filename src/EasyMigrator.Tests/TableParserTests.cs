using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    [TestFixture]
    public class TableParserTests : TableTestBase
    {
        [Test]
        public void ParseAll()
        {
            RunTest(Parse, GetAllTableDataTypes());
        }

        public void Parse(ITableTestData data)
        {
            AssertEx.AreEqual(data.Model, Parser.Default.ParseTable(data.Poco));
        }
    }
}
