using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using EasyMigrator.Tests.Schemas;
using EasyMigrator.Tests.TableTest;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    abstract public class RoundTrip : IntegrationTestBase
    {
        protected RoundTrip(Func<string, IMigrator> getMigrator) : base(getMigrator) { }

        [Test] public void Table1() => Test<Table1>();
        [Test] public void Fk1() => Test<Fk1>();

        protected override void Test(ITableTestCase testCase)
        {
            var set = Migrator.CreateMigrationSet();
            set.AddMigrationForTableTestCase(testCase);
            var mig = Migrator.CompileMigrations(set);
            Migrator.Up(mig);

            foreach (var data in testCase.Datum)
                AssertEx.AreEqual(data.Model, GetTableModelFromDb(data.Model.Name), IsMigratorDotNet);

            Migrator.Down(mig);
        }

        public class FkStuff
        {
            string Description;
        }

        public class FkCol
        {
            string Name;
        }

        public class FkColTable
        {
            [Fk("FkStuff")] int StuffId;
        }
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class RoundTrip : Tests.RoundTrip
    {
        public RoundTrip() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test]
        public void RemoveFkCol()
        {
            var set = Migrator.CreateMigrationSet();
            set.AddMigrationForFluentMigrator(
                m => {
                    m.Create.Table<FkStuff>();
                    m.Create.Table<FkCol>();
                    m.Create.Columns<FkColTable>();
                },
                m => {
                    m.Delete.Columns<FkColTable>();
                });

            var mig = Migrator.CompileMigrations(set);
            Migrator.Up(mig);
            Migrator.Down(mig);
        }
    }
}

namespace EasyMigrator.Tests.MigratorDotNet
{
    [TestFixture]
    public class RoundTrip : Tests.RoundTrip
    {
        public RoundTrip() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test]
        public void RemoveFkCol()
        {
            var set = Migrator.CreateMigrationSet();
            set.AddMigrationForMigratorDotNet(
                m => {
                    m.Database.AddTable<FkStuff>();
                    m.Database.AddTable<FkCol>();
                    m.Database.AddColumns<FkColTable>();
                },
                m => {
                    m.Database.RemoveColumns<FkColTable>();
                });

            var mig = Migrator.CompileMigrations(set);
            Migrator.Up(mig);
            Migrator.Down(mig);
        }
    }
}
