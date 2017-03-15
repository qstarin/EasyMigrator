using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseSchemaReader.DataSchema;
using EasyMigrator.MigratorDotNet;
using EasyMigrator.Tests.Integration;
using EasyMigrator.Tests.Schemas;
using EasyMigrator.Tests.TableTest;
using Migrator.Framework;
using NUnit.Framework;


namespace EasyMigrator.Tests.MigratorDotNet
{

    [TestFixture]
    public class CreateIndex : IntegrationTestBase
    {
        public CreateIndex() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test]
        public void Table1()
            => Test(null,
                    m => {
                        m.Database.AddIndex<Table1.Poco>(t => t.Name, t => t.Headline);
                    },
                    m => { },
                    idx => {
                        Assert.AreEqual(false, idx.IsUnique);
                    });

        [Test]
        public void Table1Ordered()
            => Test(null,
                    m => {
                        m.Database.AddIndex(new Descending<Table1.Poco>(t => t.Name), new Ascending<Table1.Poco>(t => t.Headline));
                    },
                    m => { },
                    idx => {
                        Assert.AreEqual(false, idx.IsUnique);
                    });

        [Test]
        public void Table1Unique()
            => Test(null,
                    m => {
                        m.Database.AddIndex<Table1.Poco>(true, t => t.Name, t => t.Headline);
                    },
                    m => { },
                    idx => {
                        // Assert.AreEqual(false, idx.IsUnique); // <- Schema reader doesn't pick this up correctly
                    });

        [Test]
        public void Table1Named()
            => Test("MyNamedIndex",
                    m => {
                        m.Database.AddIndex<Table1.Poco>("MyNamedIndex", t => t.Name, t => t.Headline);
                    },
                    m => { },
                    idx => {
                        Assert.AreEqual(false, idx.IsUnique);
                    });


        private void Test(string indexName, Action<Migration> up, Action<Migration> down, Action<DatabaseIndex> extraChecks, Action<DatabaseIndex> checkColumns = null)
        {
            var testCase = new TableTestCase<Table1>();
            var set = Migrator.CreateMigrationSet();
            set.AddMigrationForTableTestCase(testCase);
            set.AddMigrationForMigratorDotNet(up, down);

            var mig = Migrator.CompileMigrations(set);
            Migrator.Up(mig);

            var schema = GetDbSchema();
            var table = schema.FindTableByName("Table1");
            var idx = table.Indexes.Find(i => i.Name == (indexName ?? "IX_Table1_Name_Headline"));
            Assert.NotNull(idx);

            if (checkColumns == null) {
                var nameCol = idx.Columns.Find(c => c.Name == "Name");
                Assert.NotNull(nameCol);
                var headlineCol = idx.Columns.Find(c => c.Name == "Headline");
                Assert.NotNull(headlineCol);
            }
            else
                checkColumns(idx);

            extraChecks?.Invoke(idx);

            Migrator.Down(mig);
        }
    }
}
