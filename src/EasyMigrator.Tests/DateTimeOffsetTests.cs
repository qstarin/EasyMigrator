using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using NUnit.Framework;


namespace EasyMigrator.Tests.MigratorDotNet
{
    [TestFixture]
    public class DateTimeOffset : IntegrationTestBase
    {
        public DateTimeOffset() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test] public void NotNull() => Test<Schemas.DateTimeOffset.NotNull>();
        [Test] public void NotNull_AddColumns() => Test<Schemas.DateTimeOffset.NotNull.AddColumns.Empty, Schemas.DateTimeOffset.NotNull.AddColumns>();
        [Test] public void WithScale() => Test<Schemas.DateTimeOffset.WithScale>();
        [Test] public void WithScale_AddColumns() => Test<Schemas.DateTimeOffset.WithScale.AddColumns.Empty, Schemas.DateTimeOffset.WithScale.AddColumns>();
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class DateTimeOffset : IntegrationTestBase
    {
        public DateTimeOffset() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test] public void NotNull() => Test<Schemas.DateTimeOffset.NotNull>();
        [Test] public void NotNull_AddColumns() => Test<Schemas.DateTimeOffset.NotNull.AddColumns.Empty, Schemas.DateTimeOffset.NotNull.AddColumns>();
        [Test] public void WithScale() => Test<Schemas.DateTimeOffset.WithScale>();
        [Test] public void WithScale_AddColumns() => Test<Schemas.DateTimeOffset.WithScale.AddColumns.Empty, Schemas.DateTimeOffset.WithScale.AddColumns>();
    }
}
