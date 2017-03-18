using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using EasyMigrator.Tests.Schemas;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    abstract public class RoundTrip : IntegrationTestBase
    {
        protected RoundTrip(Func<string, IMigrator> getMigrator) : base(getMigrator) { }

        public class FkStuff
        {
            public string Description;
        }

        public class FkStuffTable
        {
            public int Quantity;
            [Max, NotNull] public string Story;
        }

        public class FkCol
        {
            public string Name;
        }

        public class FkColTable
        {
            [Fk("FkStuff")] public int StuffId;
            public int Case;
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
                    m.Create.Columns<FkStuffTable>(m.Alter, () => m.Update.Table(nameof(FkStuff)).Set(new { Quantity = 2, Story = "Hi" }).AllRows());
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
                    m.Database.AddIndex(new Descending<FkColTable>(t => t.Case), new Ascending<FkColTable>(t => t.StuffId));
                },
                m => {
                    m.Database.RemoveIndex<FkColTable>(t => t.Case, t => t.StuffId);
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
                    m.Database.AddColumns<FkStuffTable>(() => m.Database.Update(nameof(FkStuff), new[] { "Quantity", "Story" }, new[] { "2", "Hi" }));
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
