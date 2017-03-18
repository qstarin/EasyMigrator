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
            Assert.AreEqual(expected.PrimaryKeyName ?? $"PK_{expected.Name}", actual.PrimaryKeyName);
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
                    Assert.AreEqual(e.Precision.Precision, a.Precision.Precision);
                    Assert.AreEqual(e.Precision.Scale, a.Precision.Scale);
                }

                if (e.Index == null)
                    Assert.IsNull(a.Index);
                else {
                    Assert.AreEqual(e.Index.Name, a.Index.Name);
                    //Assert.AreEqual(e.Index.Unique, a.Index.Unique); // <- Schema reader doesn't pick this up
                    Assert.AreEqual(e.Index.Clustered, a.Index.Clustered);
                }

                if (e.ForeignKey == null)
                    Assert.IsNull(a.ForeignKey);
                else {
                    Assert.AreEqual(e.ForeignKey.Name ?? $"FK_{e.ForeignKey.Table}_{e.Name}", a.ForeignKey.Name);
                    Assert.AreEqual(e.ForeignKey.Table, a.ForeignKey.Table);
                    Assert.AreEqual(e.ForeignKey.Column, a.ForeignKey.Column);
                }
            }

            expected.CompositeIndices = expected.CompositeIndices.OrderBy(ci => ci.Name).ToList();
            actual.CompositeIndices = actual.CompositeIndices.OrderBy(ci => ci.Name).ToList();
            Assert.AreEqual(expected.CompositeIndices.Count, actual.CompositeIndices.Count);
            for (var i = 0; i < expected.CompositeIndices.Count; i++) {
                var e = expected.CompositeIndices[i];
                var a = actual.CompositeIndices[i];
                Assert.AreEqual(e.Name, a.Name);
                //Assert.AreEqual(e.Unique, a.Unique); // <- Schema reader doesn't pick this up
                Assert.AreEqual(e.Clustered, a.Clustered);

                // this is bad but schema reader doesn't get the correct order of columns
                e.Columns = e.Columns.OrderBy(ci => ci.ColumnName).ToArray();
                a.Columns = a.Columns.OrderBy(ci => ci.ColumnName).ToArray();
                Assert.AreEqual(e.Columns.Length, a.Columns.Length);
                for (var j = 0; j < e.Columns.Length; j++) {
                    var ec = e.Columns[j];
                    var ac = a.Columns[j];
                    Assert.AreEqual(ec.ColumnName, ac.ColumnName);
                    //Assert.AreEqual(ec.Direction, ac.Direction); // <- Schema reader doesn't pick this up
                }
            }
        }
    }
}
