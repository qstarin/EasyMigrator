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
    public class Table1 : IntegrationTestBase
    {
        public Table1() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }
        [Test] public void RoundTrip() => Test<Schemas.Table1>();
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class Table1 : IntegrationTestBase
    {
        public Table1() : base(s => new Integration.FluentMigrator.Migrator(s)) { }
        [Test] public void RoundTrip() => Test<Schemas.Table1>();
    }
}
