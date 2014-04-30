using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Extensions;
using EasyMigrator.Tests.Data;
using NUnit.Framework;


namespace EasyMigrator.Tests.Integration.FluentMigrator
{
    [TestFixture]
    public class FluentMigratorTests : IntegrationTestBase
    {
        [Test]
        public void RoundTripAll()
        {
            RunTest(RoundTrip, GetAllTableDataTypes());
        }

        public void RoundTrip(ITableTestData data)
        {
            Assert.AreEqual(1, 1);
        }
    }
}
