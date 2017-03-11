using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    static public class AssertEx
    {
        static public void AreEqual(Table expected, Table actual, bool isMigratorDotNet)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Columns.Count, actual.Columns.Count);
            for (var i = 0; i < expected.Columns.Count; i++) {
                var e = expected.Columns.ElementAt(i);
                var a = actual.Columns.ElementAt(i);

                Assert.AreEqual(e.Name, a.Name);
                Assert.AreEqual(e.Type, a.Type);
                Assert.AreEqual(e.DefaultValue, a.DefaultValue);
                Assert.AreEqual(e.IsNullable, a.IsNullable);
                Assert.AreEqual(e.IsPrimaryKey, a.IsPrimaryKey);
                Assert.AreEqual(e.Length, a.Length);

                if (e.AutoIncrement == null)
                    Assert.IsNull(a.AutoIncrement);
                else {
                    Assert.AreEqual(e.AutoIncrement.Seed, a.AutoIncrement.Seed);
                    Assert.AreEqual(e.AutoIncrement.Step, a.AutoIncrement.Step);
                }

                if (e.Precision == null)
                    Assert.IsNull(a.Precision);
                else {
                    Assert.AreEqual(isMigratorDotNet ? 19 : e.Precision.Precision, a.Precision.Precision);
                    Assert.AreEqual(e.Precision.Scale, a.Precision.Scale);
                }

                if (e.Index == null)
                    Assert.IsNull(a.Index);
                else {
                    //Assert.AreEqual(e.Index.Name, a.Index.Name);
                    //Assert.AreEqual(e.Index.Unique, a.Index.Unique);
                    Assert.AreEqual(e.Index.Clustered, a.Index.Clustered);
                }

                if (e.ForeignKey == null)
                    Assert.IsNull(a.ForeignKey);
                else {
                    //Assert.AreEqual(e.ForeignKey.Name, a.ForeignKey.Name);
                    Assert.AreEqual(e.ForeignKey.Table, a.ForeignKey.Table);
                    Assert.AreEqual(e.ForeignKey.Column, a.ForeignKey.Column);
                }
            }
        }
    }
}
