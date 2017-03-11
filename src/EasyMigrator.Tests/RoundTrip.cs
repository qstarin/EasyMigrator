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
            public string Description;
        }

        public class FkStuffTable
        {
            public int Quantity;
        }

        public class FkCol
        {
            public string Name;
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
        public void RemoveFkColAndPopulate()
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

            set.AddMigrationForPocoDb(
                db => {
                    db.Insert(new FkStuff { Description = "One" });
                    db.Insert(new FkStuff { Description = "Two" });
                },
                db => { db.Delete<FkStuff>("WHERE 1=1"); });

            set.AddMigrationForFluentMigrator(
                m => {
                    m.Create.Columns<FkStuffTable>(() => m.Update.Table(nameof(FkStuff)).Set(new { Quantity = 2 }).AllRows());
                    m.Alter.PostPopulate<FkStuffTable>();
                },
                m => {
                    m.Delete.Columns<FkStuffTable>();
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
        public void RemoveFkColAndPopulate()
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

            set.AddMigrationForPocoDb(
                db => {
                    db.Insert(new FkStuff { Description = "One" });
                    db.Insert(new FkStuff { Description = "Two" });
                },
                db => { db.Delete<FkStuff>("WHERE 1=1"); });

            set.AddMigrationForMigratorDotNet(
                m => {
                    m.Database.AddColumns<FkStuffTable>(() => m.Database.Update(nameof(FkStuff), new[] { "Quantity" }, new[] { "2" }));
                },
                m => {
                    m.Database.RemoveColumns<FkStuffTable>();
                });

            var mig = Migrator.CompileMigrations(set);
            Migrator.Up(mig);
            Migrator.Down(mig);
        }
    }
}
