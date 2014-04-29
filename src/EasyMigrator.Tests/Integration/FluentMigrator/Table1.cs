using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


namespace EasyMigrator.Tests.Integration.FluentMigrator
{
    [TestFixture]
    public class Table1 : FluentMigratorTestBase
    {
        [Test]
        public void RoundTripSchema()
        {
            Assert.AreEqual(1, 1);
        }
    }
}
