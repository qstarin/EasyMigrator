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
    public class StringLength : IntegrationTestBase
    {
        public StringLength() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test] public void Unspecified() => Test<StringLengths.Unspecified>();
        [Test] public void Unspecified_AddColumns() => Test<StringLengths.Unspecified.AddColumns.Empty, StringLengths.Unspecified.AddColumns.ColumnsToAdd>();
        [Test] public void Default() => Test<StringLengths.Default>();
        [Test] public void Default_AddColumns() => Test<StringLengths.Default.AddColumns.Empty, StringLengths.Default.AddColumns.ColumnsToAdd>();
        [Test] public void Short() => Test<StringLengths.Short>();
        [Test] public void Short_AddColumns() => Test<StringLengths.Short.AddColumns.Empty, StringLengths.Short.AddColumns.ColumnsToAdd>();
        [Test] public void Medium() => Test<StringLengths.Medium>();
        [Test] public void Medium_AddColumns() => Test<StringLengths.Medium.AddColumns.Empty, StringLengths.Medium.AddColumns.ColumnsToAdd>();
        [Test] public void Long() => Test<StringLengths.Long>();
        [Test] public void Long_AddColumns() => Test<StringLengths.Long.AddColumns.Empty, StringLengths.Long.AddColumns.ColumnsToAdd>();
        [Test] public void Max() => Test<StringLengths.Max>();
        [Test] public void Max_AddColumns() => Test<StringLengths.Max.AddColumns.Empty, StringLengths.Max.AddColumns.ColumnsToAdd>();
        [Test] public void Custom_500() => Test<StringLengths.Custom_500>();
        [Test] public void Custom_500_AddColumns() => Test<StringLengths.Custom_500.AddColumns.Empty, StringLengths.Custom_500.AddColumns.ColumnsToAdd>();
        [Test] public void Fixed_20() => Test<StringLengths.Fixed_20>();
        [Test] public void Fixed_20_AddColumns() => Test<StringLengths.Fixed_20.AddColumns.Empty, StringLengths.Fixed_20.AddColumns.ColumnsToAdd>();
        [Test] public void Unspecified_Ansi() => Test<StringLengths.Ansi.Unspecified>();
        [Test] public void Unspecified_Ansi_AddColumns() => Test<StringLengths.Ansi.Unspecified.AddColumns.Empty, StringLengths.Ansi.Unspecified.AddColumns.ColumnsToAdd>();
        [Test] public void Default_Ansi() => Test<StringLengths.Ansi.Default>();
        [Test] public void Default_Ansi_AddColumns() => Test<StringLengths.Ansi.Default.AddColumns.Empty, StringLengths.Ansi.Default.AddColumns.ColumnsToAdd>();
        [Test] public void Short_Ansi() => Test<StringLengths.Ansi.Short>();
        [Test] public void Short_Ansi_AddColumns() => Test<StringLengths.Ansi.Short.AddColumns.Empty, StringLengths.Ansi.Short.AddColumns.ColumnsToAdd>();
        [Test] public void Medium_Ansi() => Test<StringLengths.Ansi.Medium>();
        [Test] public void Medium_Ansi_AddColumns() => Test<StringLengths.Ansi.Medium.AddColumns.Empty, StringLengths.Ansi.Medium.AddColumns.ColumnsToAdd>();
        [Test] public void Long_Ansi() => Test<StringLengths.Ansi.Long>();
        [Test] public void Long_Ansi_AddColumns() => Test<StringLengths.Ansi.Long.AddColumns.Empty, StringLengths.Ansi.Long.AddColumns.ColumnsToAdd>();
        [Test] public void Max_Ansi() => Test<StringLengths.Ansi.Max>();
        [Test] public void Max_Ansi_AddColumns() => Test<StringLengths.Ansi.Max.AddColumns.Empty, StringLengths.Ansi.Max.AddColumns.ColumnsToAdd>();
        [Test] public void Custom_500_Ansi() => Test<StringLengths.Ansi.Custom_500>();
        [Test] public void Custom_500_Ansi_AddColumns() => Test<StringLengths.Ansi.Custom_500.AddColumns.Empty, StringLengths.Ansi.Custom_500.AddColumns.ColumnsToAdd>();
        [Test] public void Fixed_20_Ansi() => Test<StringLengths.Ansi.Fixed_20>();
        [Test] public void Fixed_20_Ansi_AddColumns() => Test<StringLengths.Ansi.Fixed_20.AddColumns.Empty, StringLengths.Ansi.Fixed_20.AddColumns.ColumnsToAdd>();
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class StringLength : IntegrationTestBase
    {
        public StringLength() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test] public void Unspecified() => Test<StringLengths.Unspecified>();
        [Test] public void Unspecified_AddColumns() => Test<StringLengths.Unspecified.AddColumns.Empty, StringLengths.Unspecified.AddColumns.ColumnsToAdd>();
        [Test] public void Default() => Test<StringLengths.Default>();
        [Test] public void Default_AddColumns() => Test<StringLengths.Default.AddColumns.Empty, StringLengths.Default.AddColumns.ColumnsToAdd>();
        [Test] public void Short() => Test<StringLengths.Short>();
        [Test] public void Short_AddColumns() => Test<StringLengths.Short.AddColumns.Empty, StringLengths.Short.AddColumns.ColumnsToAdd>();
        [Test] public void Medium() => Test<StringLengths.Medium>();
        [Test] public void Medium_AddColumns() => Test<StringLengths.Medium.AddColumns.Empty, StringLengths.Medium.AddColumns.ColumnsToAdd>();
        [Test] public void Long() => Test<StringLengths.Long>();
        [Test] public void Long_AddColumns() => Test<StringLengths.Long.AddColumns.Empty, StringLengths.Long.AddColumns.ColumnsToAdd>();
        [Test] public void Max() => Test<StringLengths.Max>();
        [Test] public void Max_AddColumns() => Test<StringLengths.Max.AddColumns.Empty, StringLengths.Max.AddColumns.ColumnsToAdd>();
        [Test] public void Custom_500() => Test<StringLengths.Custom_500>();
        [Test] public void Custom_500_AddColumns() => Test<StringLengths.Custom_500.AddColumns.Empty, StringLengths.Custom_500.AddColumns.ColumnsToAdd>();
        [Test] public void Fixed_20() => Test<StringLengths.Fixed_20>();
        [Test] public void Fixed_20_AddColumns() => Test<StringLengths.Fixed_20.AddColumns.Empty, StringLengths.Fixed_20.AddColumns.ColumnsToAdd>();
        [Test] public void Unspecified_Ansi() => Test<StringLengths.Ansi.Unspecified>();
        [Test] public void Unspecified_Ansi_AddColumns() => Test<StringLengths.Ansi.Unspecified.AddColumns.Empty, StringLengths.Ansi.Unspecified.AddColumns.ColumnsToAdd>();
        [Test] public void Default_Ansi() => Test<StringLengths.Ansi.Default>();
        [Test] public void Default_Ansi_AddColumns() => Test<StringLengths.Ansi.Default.AddColumns.Empty, StringLengths.Ansi.Default.AddColumns.ColumnsToAdd>();
        [Test] public void Short_Ansi() => Test<StringLengths.Ansi.Short>();
        [Test] public void Short_Ansi_AddColumns() => Test<StringLengths.Ansi.Short.AddColumns.Empty, StringLengths.Ansi.Short.AddColumns.ColumnsToAdd>();
        [Test] public void Medium_Ansi() => Test<StringLengths.Ansi.Medium>();
        [Test] public void Medium_Ansi_AddColumns() => Test<StringLengths.Ansi.Medium.AddColumns.Empty, StringLengths.Ansi.Medium.AddColumns.ColumnsToAdd>();
        [Test] public void Long_Ansi() => Test<StringLengths.Ansi.Long>();
        [Test] public void Long_Ansi_AddColumns() => Test<StringLengths.Ansi.Long.AddColumns.Empty, StringLengths.Ansi.Long.AddColumns.ColumnsToAdd>();
        [Test] public void Max_Ansi() => Test<StringLengths.Ansi.Max>();
        [Test] public void Max_Ansi_AddColumns() => Test<StringLengths.Ansi.Max.AddColumns.Empty, StringLengths.Ansi.Max.AddColumns.ColumnsToAdd>();
        [Test] public void Custom_500_Ansi() => Test<StringLengths.Ansi.Custom_500>();
        [Test] public void Custom_500_Ansi_AddColumns() => Test<StringLengths.Ansi.Custom_500.AddColumns.Empty, StringLengths.Ansi.Custom_500.AddColumns.ColumnsToAdd>();
        [Test] public void Fixed_20_Ansi() => Test<StringLengths.Ansi.Fixed_20>();
        [Test] public void Fixed_20_Ansi_AddColumns() => Test<StringLengths.Ansi.Fixed_20.AddColumns.Empty, StringLengths.Ansi.Fixed_20.AddColumns.ColumnsToAdd>();
    }
}
