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
    public class RoundTrip : IntegrationTestBase
    {
        public RoundTrip() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test] public void AutoInc_Custom_Byte() => Test<AutoInc_Custom_Byte>();
        [Test] public void AutoInc_Custom_Int16() => Test<AutoInc_Custom_Int16>();
        [Test] public void AutoInc_Custom_Int32() => Test<AutoInc_Custom_Int32>();
        [Test] public void AutoInc_Custom_Int64() => Test<AutoInc_Custom_Int64>();
        [Test] public void Fk_ByType_Guid() => Test<Fk_ByType_Guid>();
        [Test] public void Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>();
        [Test] public void Mtm_BookAuthors() => Test<Mtm_BookAuthors>();
        [Test] public void Table1() => Test<Table1>();

        [Test]
        public void Fk_AddToExisting() => Test<Fk_AddToExisting>(
            (testCase, migrations) => {
                AddMigrations(testCase, migrations);
                migrations.AddMigrationForMigratorDotNet(
                    m => { m.Database.AddColumns<Schemas.Fk_AddToExisting.Assoc.ColumnsToAdd>(); },
                    m => { m.Database.RemoveColumns<Schemas.Fk_AddToExisting.Assoc.ColumnsToAdd>(); });
            }
        );

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
