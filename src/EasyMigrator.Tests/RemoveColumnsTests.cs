using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using NUnit.Framework;


namespace EasyMigrator.Tests.MigratorDotNet
{
    [TestFixture]
    public class RemoveColumns : IntegrationTestBase
    {
        public RemoveColumns() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test]
        public void Pulse() => Test<Schemas.RemoveColumns>(
            (testCase, migrations) => {
                AddMigrations(testCase, migrations);

                migrations.AddMigrationForMigratorDotNet(
                    m => {
                        m.Database.RemoveColumns<Schemas.RemoveColumns.Pulse.ColumnsToRemove>();
                    },
                    m => {
                        m.Database.AddColumns<Schemas.RemoveColumns.Pulse.ColumnsToRemove>();
                    });
            }
        );
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{
    [TestFixture]
    public class RemoveColumns : IntegrationTestBase
    {
        public RemoveColumns() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test]
        public void Pulse() => Test<Schemas.RemoveColumns>(
            (testCase, migrations) => {
                AddMigrations(testCase, migrations);

                migrations.AddMigrationForFluentMigrator(
                    m => {
                        m.Delete.Columns<Schemas.RemoveColumns.Pulse.ColumnsToRemove>();
                    },
                    m => {
                        m.Create.Columns<Schemas.RemoveColumns.Pulse.ColumnsToRemove>();
                    });
            }
        );
    }
}