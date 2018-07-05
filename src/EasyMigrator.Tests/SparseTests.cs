using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.Integration;
using NUnit.Framework;


namespace EasyMigrator.Tests.MigratorDotNet
{
    [TestFixture]
    public class Sparse : IntegrationTestBase
    {
        public Sparse() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test] public void Sparse1() => Test<Schemas.Sparse>();
    }
}
