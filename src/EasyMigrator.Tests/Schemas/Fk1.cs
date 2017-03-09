using System.Data;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class Fk1 : TableTestCase
    {
        [MigrationOrder(1)] class Stuff
        {
            class Poco
            {
                [Medium] string Description;
            }

            Table Model = new Table {
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
                        IsNullable = true,
                        Length = 255
                    }
                }
            };
        }

        [MigrationOrder(2)] class Assoc
        {
            class Poco
            {
                [Fk("Stuff")] int StuffId;
                [Fk("Stuff")] int AltStuffId;
            }

            Table Model = new Table {
                Name = "Assoc",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "StuffId",
                        Type = DbType.Int32,
                        ForeignKey = new FkAttribute("Stuff") { Column = "Id"},
                        Index = new IndexAttribute()
                    },
                    new Column {
                        Name = "AltStuffId",
                        Type = DbType.Int32,
                        ForeignKey = new FkAttribute("Stuff") { Column = "Id"},
                        Index = new IndexAttribute()
                    }
                }
            };
        }
    }
}
