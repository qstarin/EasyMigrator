using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Extensions;
using EasyMigrator.Parsing;
using EasyMigrator.Tests.Schemas;
using EasyMigrator.Tests.TableTest;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    [TestFixture]
    public class Parse : TableTestBase
    {
        [Test] public void AutoInc_Custom_Byte() => Test<AutoInc_Custom_Byte>();
        [Test] public void AutoInc_Custom_Int16() => Test<AutoInc_Custom_Int16>();
        [Test] public void AutoInc_Custom_Int32() => Test<AutoInc_Custom_Int32>();
        [Test] public void AutoInc_Custom_Int64() => Test<AutoInc_Custom_Int64>();
        [Test] public void DateTime2_WithScale() => Test<DateTime2.WithScale>();
        [Test] public void Fk_ByType_Guid() => Test<Fk_ByType_Guid>();
        [Test] public void Fk_MultipleToSameTable_Int32() => Test<Fk_MultipleToSameTable_Int32>();
        [Test] public void Fk_Not_Indexed() => Test<Fk_Not_Indexed>();
        [Test] public void Mtm_BookAuthors() => Test<Mtm_BookAuthors>();
        [Test] public void Precision_Unspecified() => Test<Precisions.Unspecified>();
        [Test] public void Precision_Default() => Test<Precisions.Default>();
        [Test] public void Precision_Short() => Test<Precisions.Short>();
        [Test] public void Precision_Medium() => Test<Precisions.Medium>();
        [Test] public void Precision_Long() => Test<Precisions.Long>();
        [Test] public void Precision_Max() => Test<Precisions.Max>();
        [Test] public void Precision_Custom_13_9() => Test<Precisions.Custom_13_9>();
        [Test] public void SelfReferential() => Test<SelfReferential>();
        [Test] public void Sparse() => Test<Sparse>();
        [Test] public void StringLength_Unspecified() => Test<StringLengths.Unspecified>();
        [Test] public void StringLength_Default() => Test<StringLengths.Default>();
        [Test] public void StringLength_Short() => Test<StringLengths.Short>();
        [Test] public void StringLength_Medium() => Test<StringLengths.Medium>();
        [Test] public void StringLength_Long() => Test<StringLengths.Long>();
        [Test] public void StringLength_Max() => Test<StringLengths.Max>();
        [Test] public void StringLength_Custom_500() => Test<StringLengths.Custom_500>();
        [Test] public void StringLength_Fixed_20() => Test<StringLengths.Fixed_20>();
        [Test] public void StringLength_Unspecified_Ansi() => Test<StringLengths.Ansi.Unspecified>();
        [Test] public void StringLength_Default_Ansi() => Test<StringLengths.Ansi.Default>();
        [Test] public void StringLength_Short_Ansi() => Test<StringLengths.Ansi.Short>();
        [Test] public void StringLength_Medium_Ansi() => Test<StringLengths.Ansi.Medium>();
        [Test] public void StringLength_Long_Ansi() => Test<StringLengths.Ansi.Long>();
        [Test] public void StringLength_Max_Ansi() => Test<StringLengths.Ansi.Max>();
        [Test] public void StringLength_Custom_500_Ansi() => Test<StringLengths.Ansi.Custom_500>();
        [Test] public void StringLength_Fixed_20_Ansi() => Test<StringLengths.Ansi.Fixed_20>();
        [Test] public void Table1() => Test<Table1>();

        protected override void Test(ITableTestCase testCase)
        {
            foreach (var data in testCase.Datum)
                AssertEx.AreEqual(data.Model, data.Poco.ParseTable().Table, false, false);
        }
    }
}
