using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class Fk_AddToExisting : TableTestCase
    {
        [MigrationOrder(1)]
        class Stuff
        {
            class Poco
            {
                [Medium] string Description;
            }

            static Table Model = new Table {
                Name = "Stuff",
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
        public class Assoc
        {
            class Poco
            {
                [Medium] string Description;
            }

            static Table Model = new Table {
                Name = "Assoc",
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
                        Name = "StuffId",
                        Type = DbType.Int32,
                        ForeignKey = new FkAttribute("Stuff") { Column = "Id", OnDelete = Rule.Cascade, OnUpdate = Rule.Cascade },
                    },
                },
                Indices = new List<IIndex> {
                    new Index {
                        Name = "IX_Assoc_StuffId",
                        Unique = false,
                        Clustered = false,
                        Columns = new [] { new IndexColumn("StuffId") }
                    },
                },
            };
        }

        public class ColumnsToAdd
        {
            [MigrationOrder(3)]
            public class Assoc
            {
                public class Poco
                {
                    [Fk("Stuff", OnDelete = Rule.Cascade, OnUpdate = Rule.Cascade)] public int StuffId;
                }
            }
        }
    }
}
