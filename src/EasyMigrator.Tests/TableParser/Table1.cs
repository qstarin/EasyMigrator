using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Model;
using NUnit.Framework;


namespace EasyMigrator.Tests.TableParser
{
    [TestFixture]
    public class Table1
    {
        [Test]
        public void Parser()
        {
            var table = Parsing.Parser.Default.ParseTable(typeof(Table1Table));
            AssertExtensions.AreEqual(_Table1, table);
        }

        public class Table1Table
        {
            int Sequence;
            bool Accepted;
            [NotNull] string Name;
            [FixedLength(8), Ansi, NotNull, Unique(Name = "idx_code")] string Code;
            [Length(Length.Medium)] string Headline;
            [Max] string Description;
            [Precision(2, 18)] decimal? Rate;
            [Fk("OtherTable")] int OtherTableId;
        }

        private Table _Table1 = new Table {
            Name = "Table1",
            Columns = new[] {
                new Column {
                    Name = "Id",
                    Type = DbType.Int32,
                    IsPrimaryKey = true,
                    AutoIncrement = new AutoIncAttribute()
                },
                new Column {
                    Name = "Sequence",
                    Type = DbType.Int32,
                },
                new Column {
                    Name = "Accepted",
                    Type = DbType.Boolean,
                },
                new Column {
                    Name = "Name",
                    Type = DbType.String,
                    Length = 50
                },
                new Column {
                    Name = "Code",
                    Type = DbType.AnsiStringFixedLength,
                    Length = 8,
                    Index = new IndexAttribute {Name = "idx_code", Unique = true}
                },
                new Column {
                    Name = "Headline",
                    Type = DbType.String,
                    IsNullable = true,
                    Length = 255
                },
                new Column {
                    Name = "Description",
                    Type = DbType.String,
                    IsNullable = true,
                    Length = int.MaxValue
                },
                new Column {
                    Name = "Rate",
                    Type = DbType.Decimal,
                    IsNullable = true,
                    Precision = new PrecisionAttribute(2, 18)
                },
                new Column {
                    Name = "OtherTableId",
                    Type = DbType.Int32,
                    ForeignKey = new FkAttribute("OtherTable"),
                    Index = new IndexAttribute()
                },
            }
        };
    }
}
