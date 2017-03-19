using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using EasyMigrator.Tests.Schemas;
using NUnit.Framework;


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class RoundTrip : IntegrationTestBase
    {
        public RoundTrip() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test] public void AutoInc_Custom_Byte() => Test<AutoInc_Custom_Byte>();
        [Test] public void AutoInc_Custom_Int16() => Test<AutoInc_Custom_Int16>();
        [Test] public void AutoInc_Custom_Int32() => Test<AutoInc_Custom_Int32>();
        [Test] public void AutoInc_Custom_Int64() => Test<AutoInc_Custom_Int64>();
        [Test] public void Fk_AddToExisting() => Test<Schemas.Fk_AddToExisting, Schemas.Fk_AddToExisting.ColumnsToAdd>();
        [Test] public void Fk_ByType_Guid() => Test<Fk_ByType_Guid>();
        [Test] public void Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>();
        [Test] public void Mtm_BookAuthors() => Test<Mtm_BookAuthors>();
        [Test] public void Table1() => Test<Table1>();

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
