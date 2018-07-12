using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class SelfReferential
    {
        public class ParentOfSelf
        {
            public class Poco
            {
                [Fk(typeof(Poco))] public int? ParentId;
            }

            static Table Model = new Table {
                Name = "ParentOfSelf",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "ParentId",
                        Type = DbType.Int32,
                        IsNullable = true,
                        ForeignKey = new FkAttribute("ParentOfSelf") { Column = "Id" },
                    },
                },
                Indices = new List<IIndex> {
                    new Index {
                        Name = "IX_ParentOfSelf_ParentId",
                        Unique = false,
                        Clustered = false,
                        Columns = new [] { new IndexColumn("ParentId") }
                    },
                },
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(ParentOfSelf))] public class Poco { }
                    static Table Model = ParentOfSelf.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(ParentOfSelf))]
                    public class Poco
                    {
                        [Fk(typeof(Poco))] public int? ParentId;
                    }
                }
            }
        }
    }
}
