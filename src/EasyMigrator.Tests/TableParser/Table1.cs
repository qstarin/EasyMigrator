using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


namespace EasyMigrator.Tests.TableParser
{
    [TestFixture]
    public class Table1
    {
        [Test]
        public void Parses()
        {
            var table = Parsing.Parser.Default.ParseTable(typeof(Table1Table));
            Assert.AreEqual("Table1", table.Name);
        }

        public class Table1Table
        {
        }
    }
}
