using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class Fk_ByType_Guid : TableTestCase
    {
        [MigrationOrder(1)]
        public class Master
        {
            public class Poco
            {
                [Pk] public Guid Id;
            }

            static Table Model = new Table {
                Name = "Master",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Guid,
                        IsPrimaryKey = true,
                    },
                }
            };
        }

        [MigrationOrder(2)]
        public class Slave
        {
            public class Poco
            {
                [Pk] public Guid Id;
                [Fk(typeof(Master), OnDelete = Rule.None)] public Guid MasterId;
            }

            static Table Model = new Table {
                Name = "Slave",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Guid,
                        IsPrimaryKey = true,
                    },
                    new Column {
                        Name = "MasterId",
                        Type = DbType.Guid,
                        ForeignKey = new FkAttribute("Master") {
                            Name = "FK_Slave_MasterId",
                            Column = "Id",
                            Indexed = true,
                            OnDelete = Rule.None,
                        },
                    },
                },
                Indices = new List<IIndex> {
                    new Index {
                        Name = "IX_Slave_MasterId",
                        Unique = false,
                        Clustered = false,
                        Columns = new [] { new IndexColumn("MasterId") }
                    },
                },
            };
        }
    }
}
