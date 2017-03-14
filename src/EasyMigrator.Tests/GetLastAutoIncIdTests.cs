using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.MigratorDotNet;
using EasyMigrator.Tests.Integration;
using EasyMigrator.Tests.Schemas;
using EasyMigrator.Tests.TableTest;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    abstract public class GetLastAutoIncId : IntegrationTestBase
    {
        protected GetLastAutoIncId(Func<string, IMigrator> getMigrator) : base(getMigrator) { }

        [Test] public void Fk1() => Test<Fk1>();

        protected override void Test(ITableTestCase testCase)
        {
            var set = Migrator.CreateMigrationSet();
            set.AddMigrationForTableTestCase(testCase);
            AddMigrations(set);
            var mig = Migrator.CompileMigrations(set);
            Migrator.Up(mig);
            Migrator.Down(mig);
        }

        abstract protected void AddMigrations(IMigrationSet migrations);
    }
}

namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class GetLastAutoIncId : Tests.GetLastAutoIncId
    {
        public GetLastAutoIncId() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        protected override void AddMigrations(IMigrationSet migrations)
        {
            migrations.AddMigrationForMigratorDotNet(
                m => {
                    m.Database.Insert("Stuff", new[] { "Description" }, new[] { "One" });
                    m.Database.Insert("Stuff", new[] { "Description" }, new[] { "Two" });
                    m.Database.Insert("Stuff", new[] { "Description" }, new[] { "Three" });
                    var lastId = m.Database.GetLastAutoIncrementInt32();
                    Assert.AreEqual(3, lastId);
                }, 
                m => { });
        }
    }
}