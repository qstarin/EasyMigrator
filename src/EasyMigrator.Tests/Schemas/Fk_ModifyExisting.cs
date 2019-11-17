using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class Fk_ModifyExisting : TableTestCase
    {
        [MigrationOrder(1)]
        class Select
        {
            class Poco
            {
                [Medium] string Description;
            }

            static Table Model = new Table {
                Name = "Select",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Description",
                        Type = DbType.String,
                        Length = 255,
                        IsNullable = true,
                    },
                }
            };
        }

        [MigrationOrder(2)]
        public class Alias
        {
            class Poco
            {
                [Medium] string Description;
                public int SelectId;
            }

            static Table Model = new Table {
                Name = "Alias",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Description",
                        Type = DbType.String,
                        Length = 255,
                        IsNullable = true,
                    },
                    new Column {
                        Name = "SelectId",
                        Type = DbType.Int32,
                        ForeignKey = new FkAttribute("Select") { Column = "Id", OnDelete = Rule.None, OnUpdate = Rule.None },
                    },
                },
                Indices = new List<IIndex> {
                    new Index {
                        Name = "IX_Alias_SelectId",
                        Unique = false,
                        Clustered = false,
                        Columns = new [] { new IndexColumn("SelectId") }
                    },
                },
            };
        }

        [Name("Alias")]
        public class ColumnsToChange
        {
            [Fk("Select", OnDelete = Rule.Cascade, OnUpdate = Rule.Cascade)] public int SelectId;
        }
    }
}
