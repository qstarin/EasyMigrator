using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


namespace EasyMigrator.Tests.Integration
{
    abstract public class IntegrationTests : IntegrationTestBase
    {
        protected IntegrationTests(Func<string, IMigrator> getMigrator) : base(getMigrator) {}

        [Test]
        public void ParseAll()
        {
            RunTest(RoundTrip, GetAllTableDataTypes());
        }

        protected void RoundTrip(ITableTestData data)
        {
            Migrator.Up(data.Poco);
            AssertEx.AreEqual(data.Model, GetTableModelFromDb(data));
            Migrator.Down(data.Poco);
        }
    }

    [TestFixture]
    public class FluentMigratorIntegrationTests : IntegrationTests
    {
       public FluentMigratorIntegrationTests() : base(s => new Migrators.FluentMigrator(s)) {}
    }

    [TestFixture]
    public class MigratorDotNetIntegrationTests : IntegrationTests
    {
        public MigratorDotNetIntegrationTests() : base(s => new Migrators.FluentMigrator(s)) { }
    }
}
