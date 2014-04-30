using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


namespace EasyMigrator.Tests.Integration
{
    abstract public class IntegrationTests : IntegrationTestBase
    {
        [Test]
        public void ParseAll()
        {
            RunTest(RoundTrip, GetAllTableDataTypes());
        }

        protected void RoundTrip(ITableTestData data)
        {
            Migrator.Up(data.Poco);
            AssertEx.AreEqual(data.Model, GetTableModelFromDb(data));
        }
    }

    [TestFixture]
    public class FluentMigratorIntegrationTests : IntegrationTests
    {
        protected override IMigrator Migrator { get { return new Migrators.FluentMigrator(); } }
    }

    [TestFixture]
    public class MigratorDotNetIntegrationTests : IntegrationTests
    {
        protected override IMigrator Migrator { get { return new Migrators.MigratorDotNet(); } }
    }
}
