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
    public class ForeignKey : IntegrationTestBase
    {
        public ForeignKey() : base(s => new Integration.MigratorDotNet.Migrator(s)) { }

        [Test] public void Fk_AddToExisting() => Test<Schemas.Fk_AddToExisting, Schemas.Fk_AddToExisting.ColumnsToAdd>();
        [Test] public void Fk_ByType_Guid() => Test<Fk_ByType_Guid>();
        [Test] public void Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>();
        [Test] public void SelfReferential() => Test<SelfReferential.ParentOfSelf>();
        [Test] public void SelfReferential_AddColumns() => Test<SelfReferential.ParentOfSelf.AddColumns.Empty, SelfReferential.ParentOfSelf.AddColumns.ColumnsToAdd>();
    }
}


namespace EasyMigrator.Tests.FluentMigrator
{

    [TestFixture]
    public class ForeignKey : IntegrationTestBase
    {
        public ForeignKey() : base(s => new Integration.FluentMigrator.Migrator(s)) { }

        [Test] public void Fk_AddToExisting() => Test<Schemas.Fk_AddToExisting, Schemas.Fk_AddToExisting.ColumnsToAdd>();
        [Test] public void Fk_ByType_Guid() => Test<Fk_ByType_Guid>();
        [Test] public void Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>();
        [Test] public void SelfReferential() => Test<SelfReferential>();
        [Test] public void SelfReferential_AddColumns() => Test<SelfReferential.ParentOfSelf.AddColumns.Empty, SelfReferential.ParentOfSelf.AddColumns.ColumnsToAdd>();
    }
}
