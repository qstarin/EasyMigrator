using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class Fk_Not_Indexed
    {
        [MigrationOrder(1)]
        public class Master
        {
            
            public class Poco { }

            static Table Model = new Table {
                Name = "Master",
                PrimaryKeyName = "PK_Master",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                }
            };
        }

        [MigrationOrder(2)]
        public class Sub
        {
            public class Poco
            {
                [Fk(typeof(Master), Indexed = false)] public int MasterId;
            }

            static Table Model = new Table {
                Name = "Sub",
                PrimaryKeyName = "PK_Sub",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "MasterId",
                        Type = DbType.Int32,
                        ForeignKey = new FkAttribute("Master") { Column = "Id", Name = "FK_Sub_MasterId" },
                    },
                }
            };
        }
    }
}
