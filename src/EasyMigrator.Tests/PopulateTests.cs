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
    public class Populate : IntegrationTestBase
    {
        public Populate() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }


        [Test]
        public void AddColumns_WithPopulate_IntAndString() => Test<AddColumns_WithPopulate_IntAndString>(
            (testCase, migrations) => {
                AddMigrations(testCase, migrations);

                migrations.AddMigrationForPocoDb(
                    db => {
                        db.Insert(new Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.Poco { Name = "One" });
                        db.Insert(new Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.Poco { Name = "Two" });
                    },
                    db => { db.Delete<Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.Poco>("WHERE 1=1"); });

                migrations.AddMigrationForMigratorDotNet(
                    m => {
                        m.Database.AddColumns<Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.ColumnsToAdd>(
                            () => m.Database.Update(
                                nameof(Schemas.AddColumns_WithPopulate_IntAndString.AddColumns),
                                new[] { "Quantity", "Story" }, 
                                new[] { "2", "Hi" }));
                    },
                    m => {
                        m.Database.RemoveColumns<Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.ColumnsToAdd>();
                    });
            }
        );
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class Populate : IntegrationTestBase
    {
        public Populate() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test]
        public void AddColumns_WithPopulate_IntAndString() => Test<Schemas.AddColumns_WithPopulate_IntAndString>(
            (testCase, migrations) => {
                AddMigrations(testCase, migrations);

                migrations.AddMigrationForPocoDb(
                    db => {
                        db.Insert(new Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.Poco { Name = "One" });
                        db.Insert(new Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.Poco { Name = "Two" });
                    },
                    db => { db.Delete<Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.Poco>("WHERE 1=1"); });

                migrations.AddMigrationForFluentMigrator(
                    m => {
                        m.Create.Columns<Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.ColumnsToAdd>(m.Alter,
                            () => m.Update
                                   .Table(nameof(Schemas.AddColumns_WithPopulate_IntAndString.AddColumns))
                                   .Set(new { Quantity = 2, Story = "Hi" })
                                   .AllRows());
                    },
                    m => {
                        m.Delete.Columns<Schemas.AddColumns_WithPopulate_IntAndString.AddColumns.ColumnsToAdd>();
                    });
            }
        );
    }
}
