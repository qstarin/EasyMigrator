using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using NUnit.Framework;


namespace EasyMigrator.Tests.MigratorDotNet
{
    [TestFixture]
    public class ChangeColumns : IntegrationTestBase
    {
        public ChangeColumns() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test]
        public void Content() => Test<Schemas.ChangeColumns.Content>(
            (testCase, migrations) => {
                AddMigrations(testCase, migrations);

                migrations.AddMigrationForMigratorDotNet(
                    m => {
                        m.Database.ChangeColumns<Schemas.ChangeColumns.Content.ColumnsToChange>();
                    },
                    m => {
                        m.Database.ChangeColumns<Schemas.ChangeColumns.Content.OriginalColumns>();
                    });
            }
        );
    }
}

