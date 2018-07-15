using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class Table1 : TableTestCase
    {
        public class Poco
        {
            public int Sequence;
            [Default("1")] public bool Accepted;
            [NotNull] public string Name;
            [Fixed(8), Ansi, NotNull, Unique(Name = "idx_code"), Default("'ABCD'")] public string Code;
            [Length(Length.Medium)] public string Headline;
            [Max] public string Desc;
            [DbType(DbType.Currency)] public decimal? Rate;
            [Precision(Length.Short, 3), Default("1")] public decimal Adjustment;
            [Sparse, Fixed(16)] public byte[] IPv6;
            [DbType("MONEY")] public double Cost;
            [Default("GETDATE()")] public DateTime CreatedOn;
            [Default("'2018-01-01'")] public DateTime ExpiresOn;

            Index<Poco> i1 = new Index<Poco>(x => x.Name, x => x.Code) { Unique = true, Where = $"{nameof(Name)} IS NOT NULL", With = "PAD_INDEX=ON" };
            [Unique(With = "SORT_IN_TEMPDB=OFF")] Index<Poco> i2 = new Index<Poco>(new Descending<Poco>(x => x.Sequence), new Ascending<Poco>(x => x.Code)) { Name = "IX_Custom_Name" };
        }

        static Table Model = new Table {
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
                    DefaultValue = "1",
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
                    DefaultValue = "'ABCD'",
                },
                new Column {
                    Name = "Headline",
                    Type = DbType.String,
                    IsNullable = true,
                    Length = 255
                },
                new Column {
                    Name = "Desc",
                    Type = DbType.String,
                    IsNullable = true,
                    Length = int.MaxValue
                },
                new Column {
                    Name = "Rate",
                    Type = DbType.Currency,
                    IsNullable = true,
                    //Precision = new PrecisionAttribute(19, 2)
                },
                new Column {
                    Name = "Adjustment",
                    Type = DbType.Decimal,
                    Precision = new PrecisionAttribute(9, 3),
                    DefaultValue = "1",
                },
                new Column {
                    Name = "IPv6",
                    Type = DbType.Binary,
                    IsNullable = true,
                    IsSparse = true,
                    Length = 16,
                },
                new Column {
                    Name = "Cost",
                    Type = DbType.Currency,
                    CustomType = "MONEY",
                },
                new Column {
                    Name = "CreatedOn",
                    Type = DbType.DateTime,
                    DefaultValue = "GETDATE()",
                },
                new Column {
                    Name = "ExpiresOn",
                    Type = DbType.DateTime,
                    DefaultValue = "'2018-01-01'",
                },
            },
            Indices = new List<IIndex> {
                new Index {
                    Name = "IX_Table1_Name_Code",
                    Unique = true,
                    Clustered = false,
                    Where ="Name IS NOT NULL",
                    With = "PAD_INDEX=ON",
                    Columns = new [] {
                        new IndexColumn("Name"),
                        new IndexColumn("Code"),
                    }
                },
                new Index {
                    Name = "IX_Custom_Name",
                    Unique = true,
                    Clustered = false,
                    With = "SORT_IN_TEMPDB=OFF",
                    Columns = new [] {
                        new IndexColumn("Sequence", SortOrder.Descending),
                        new IndexColumn("Code", SortOrder.Ascending),
                    }
                },
                new Index {
                    Name = "idx_code",
                    Unique = true,
                    Clustered = false,
                    Columns = new [] { new IndexColumn("Code"), }
                },
            },
        };
    }
}
