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
            public bool Accepted;
            [NotNull] public string Name;
            [Fixed(8), Ansi, NotNull, Unique(Name = "idx_code")] public string Code;
            [Length(Length.Medium)] public string Headline;
            [Max] public string Desc;
            [DbType(DbType.Currency)] public decimal? Rate;
            [Precision(Length.Short, 3)] public decimal Adjustment;

            CompositeIndex index1 = new CompositeIndex("Sequence", "Headline DESC");
            CompositeIndex index2 = new CompositeIndex(new IndexColumn("Adjustment"), new IndexColumn("Name"));
            CompositeIndex index3 = new CompositeIndex(new Descending("Adjustment"), new Ascending("Headline"));
            CompositeIndex<Poco> i1 = new CompositeIndex<Poco>(x => x.Name, x => x.Code);
            [Unique(Name = "IX_Custom_Name")] CompositeIndex<Poco> i2 = new CompositeIndex<Poco>(new Descending<Poco>(x => x.Sequence), new Ascending<Poco>(x => x.Code));
        }

        static Table Model = new Table {
            Name = "Table1",
            CompositeIndices = new List<Parsing.Model.CompositeIndex> {
                new Parsing.Model.CompositeIndex {
                    Name = "IX_Table1_Name_Code",
                    Unique = false,
                    Clustered = false,
                    Columns = new [] {
                        new IndexColumn("Name"),
                        new IndexColumn("Code"),
                    }
                },
                new Parsing.Model.CompositeIndex {
                    Name = "IX_Custom_Name",
                    Unique = true,
                    Clustered = false,
                    Columns = new [] {
                        new IndexColumn("Sequence", SortOrder.Descending),
                        new IndexColumn("Code", SortOrder.Ascending),
                    }
                },
            },
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
                    DefaultValue = "0",
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
                    Index = new UniqueAttribute { Name = "idx_code" }
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
                    Precision = new PrecisionAttribute(9, 3)
                },
            }
        };
    }
}
