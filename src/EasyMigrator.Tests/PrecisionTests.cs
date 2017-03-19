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
    public class Precision : IntegrationTestBase
    {
        public Precision() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test] public void Unspecified() => Test<Precisions.Unspecified>();
        [Test] public void Unspecified_AddColumns() => Test<Precisions.Unspecified.AddColumns.Empty, Precisions.Unspecified.AddColumns.ColumnsToAdd>();
        [Test] public void Default() => Test<Precisions.Default>();
        [Test] public void Default_AddColumns() => Test<Precisions.Default.AddColumns.Empty, Precisions.Default.AddColumns.ColumnsToAdd>();
        [Test] public void Short() => Test<Precisions.Short>();
        [Test] public void Short_AddColumns() => Test<Precisions.Short.AddColumns.Empty, Precisions.Short.AddColumns.ColumnsToAdd>();
        [Test] public void Medium() => Test<Precisions.Medium>();
        [Test] public void Medium_AddColumns() => Test<Precisions.Medium.AddColumns.Empty, Precisions.Medium.AddColumns.ColumnsToAdd>();
        [Test] public void Long() => Test<Precisions.Long>();
        [Test] public void Long_AddColumns() => Test<Precisions.Long.AddColumns.Empty, Precisions.Long.AddColumns.ColumnsToAdd>();
        [Test] public void Max() => Test<Precisions.Max>();
        [Test] public void Max_AddColumns() => Test<Precisions.Max.AddColumns.Empty, Precisions.Max.AddColumns.ColumnsToAdd>();
        [Test] public void Custom_13_9() => Test<Precisions.Custom_13_9>();
        [Test] public void Custom_13_9_AddColumns() => Test<Precisions.Custom_13_9.AddColumns.Empty, Precisions.Custom_13_9.AddColumns.ColumnsToAdd>();
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class Precision : IntegrationTestBase
    {
        public Precision() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test] public void Unspecified() => Test<Precisions.Unspecified>();
        [Test] public void Unspecified_AddColumns() => Test<Precisions.Unspecified.AddColumns.Empty, Precisions.Unspecified.AddColumns.ColumnsToAdd>();
        [Test] public void Default() => Test<Precisions.Default>();
        [Test] public void Default_AddColumns() => Test<Precisions.Default.AddColumns.Empty, Precisions.Default.AddColumns.ColumnsToAdd>();
        [Test] public void Short() => Test<Precisions.Short>();
        [Test] public void Short_AddColumns() => Test<Precisions.Short.AddColumns.Empty, Precisions.Short.AddColumns.ColumnsToAdd>();
        [Test] public void Medium() => Test<Precisions.Medium>();
        [Test] public void Medium_AddColumns() => Test<Precisions.Medium.AddColumns.Empty, Precisions.Medium.AddColumns.ColumnsToAdd>();
        [Test] public void Long() => Test<Precisions.Long>();
        [Test] public void Long_AddColumns() => Test<Precisions.Long.AddColumns.Empty, Precisions.Long.AddColumns.ColumnsToAdd>();
        [Test] public void Max() => Test<Precisions.Max>();
        [Test] public void Max_AddColumns() => Test<Precisions.Max.AddColumns.Empty, Precisions.Max.AddColumns.ColumnsToAdd>();
        [Test] public void Custom_13_9() => Test<Precisions.Custom_13_9>();
        [Test] public void Custom_13_9_AddColumns() => Test<Precisions.Custom_13_9.AddColumns.Empty, Precisions.Custom_13_9.AddColumns.ColumnsToAdd>();
    }
}
