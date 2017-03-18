using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using EasyMigrator.Tests.Schemas;
using EasyMigrator.Tests.TableTest;
using NUnit.Framework;


namespace EasyMigrator.Tests.MigratorDotNet
{

    [TestFixture]
    public class GetLastAutoIncId : IntegrationTestBase
    {
        public GetLastAutoIncId() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test]
        public void Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>(
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
    public class GetLastAutoIncId : IntegrationTestBase
    {
        public GetLastAutoIncId() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test]
        public void Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>(
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
