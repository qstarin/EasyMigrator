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
    public class ManyToMany : IntegrationTestBase
    {
        public ManyToMany() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test] public void Mtm_BookAuthors() => Test<Mtm_BookAuthors>();
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class ManyToMany : IntegrationTestBase
    {
        public ManyToMany() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test] public void Mtm_BookAuthors() => Test<Mtm_BookAuthors>();
    }
}
