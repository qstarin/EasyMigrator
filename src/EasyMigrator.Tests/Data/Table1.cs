using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Tests.Data
{
    public class Table1 : TableTestCase
    {
        class Poco
        {
            int Sequence;
            bool Accepted;
            [NotNull] string Name;
            [Fixed(8), Ansi, NotNull, Unique(Name = "idx_code")] string Code;
            [Length(Length.Medium)] string Headline;
            [Max] string Description;
            decimal? Rate;
            [Precision(Length.Short, 3)] decimal Adjustment;
        }

        Table Model = new Table {
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
                    Precision = new PrecisionAttribute(19, 2)
                },
                new Column {
                    Name = "Adjustment",
                    Type = DbType.Decimal,
                    Precision = new PrecisionAttribute(9, 3)
                },
            }
        };
    }
}
