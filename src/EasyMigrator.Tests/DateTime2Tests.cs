using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using NUnit.Framework;


namespace EasyMigrator.Tests.MigratorDotNet
{
    [TestFixture]
    public class DateTime2 : IntegrationTestBase
    {
        public DateTime2() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test] public void NotNull() => Test<Schemas.DateTime2.NotNull>();
        [Test] public void NotNull_AddColumns() => Test<Schemas.DateTime2.NotNull.AddColumns.Empty, Schemas.DateTime2.NotNull.AddColumns>();
        [Test] public void WithScale() => Test<Schemas.DateTime2.WithScale>();
        [Test] public void WithScale_AddColumns() => Test<Schemas.DateTime2.WithScale.AddColumns.Empty, Schemas.DateTime2.WithScale.AddColumns>();
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class DateTime2 : IntegrationTestBase
    {
        public DateTime2() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test] public void NotNull() => Test<Schemas.DateTime2.NotNull>();
        [Test] public void NotNull_AddColumns() => Test<Schemas.DateTime2.NotNull.AddColumns.Empty, Schemas.DateTime2.NotNull.AddColumns>();
        [Test] public void WithScale() => Test<Schemas.DateTime2.WithScale>();
        [Test] public void WithScale_AddColumns() => Test<Schemas.DateTime2.WithScale.AddColumns.Empty, Schemas.DateTime2.WithScale.AddColumns>();
    }
}
