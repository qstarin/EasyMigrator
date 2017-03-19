using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using EasyMigrator.Tests.Schemas;
using NUnit.Framework;


namespace EasyMigrator.Tests.MigratorDotNet
{
    [TestFixture]
    public class AutoInc : IntegrationTestBase
    {
        public AutoInc() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test] public void Custom_Byte() => Test<AutoInc_Custom_Byte>();
        [Test] public void Custom_Int16() => Test<AutoInc_Custom_Int16>();
        [Test] public void Custom_Int32() => Test<AutoInc_Custom_Int32>();
        [Test] public void Custom_Int64() => Test<AutoInc_Custom_Int64>();

        [Test]
        public void GetLastId_Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>(
            (testCase, migrations) => {
                AddMigrations(testCase, migrations);
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
        );
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class AutoInc : IntegrationTestBase
    {
        public AutoInc() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test] public void Custom_Byte() => Test<AutoInc_Custom_Byte>();
        [Test] public void Custom_Int16() => Test<AutoInc_Custom_Int16>();
        [Test] public void Custom_Int32() => Test<AutoInc_Custom_Int32>();
        [Test] public void Custom_Int64() => Test<AutoInc_Custom_Int64>();

        [Test]
        public void GetLastId_Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>(
            (testCase, migrations) => {
                AddMigrations(testCase, migrations);
                migrations.AddMigrationForFluentMigrator(
                    m => {
                        m.Insert.IntoTable("Stuff")
                                .Row(new { Description = "One" })
                                .Row(new { Description = "Two" })
                                .Row(new { Description = "Three" });

                        m.GetLastAutoIncrementInt32(id => {
                            Assert.AreEqual(3, id);
                        });
                    },
                    m => { });
            }
        );
    }
}
