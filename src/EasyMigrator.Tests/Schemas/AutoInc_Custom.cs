using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class AutoInc_Custom_Byte : TableTestCase
    {
        public class Poco
        {
            [Pk, AutoInc(10, 3)] public byte Id;
        }

        static Table Model = new Table {
            Name = "AutoInc_Custom_Byte",
            Columns = new[] {
                new Column {
                    Name = "Id",
                    Type = DbType.Byte,
                    IsPrimaryKey = true,
                    AutoIncrement = new AutoIncAttribute(10, 3)
                },
            },
        };
    }

    public class AutoInc_Custom_Int16 : TableTestCase
    {
        public class Poco
        {
            [Pk, AutoInc(10, 3)] public short Id;
        }

        static Table Model = new Table {
            Name = "AutoInc_Custom_Int16",
            Columns = new[] {
                new Column {
                    Name = "Id",
                    Type = DbType.Int16,
                    IsPrimaryKey = true,
                    AutoIncrement = new AutoIncAttribute(10, 3)
                },
            },
        };
    }

    public class AutoInc_Custom_Int32 : TableTestCase
    {
        public class Poco
        {
            [Pk, AutoInc(10, 3)] public int Id;
        }

        static Table Model = new Table {
            Name = "AutoInc_Custom_Int32",
            Columns = new[] {
                new Column {
                    Name = "Id",
                    Type = DbType.Int32,
                    IsPrimaryKey = true,
                    AutoIncrement = new AutoIncAttribute(10, 3)
                },
            },
        };
    }

    public class AutoInc_Custom_Int64 : TableTestCase
    {
        public class Poco
        {
            [Pk, AutoInc(10, 3)] public long Id;
        }

        static Table Model = new Table {
            Name = "AutoInc_Custom_Int64",
            Columns = new[] {
                new Column {
                    Name = "Id",
                    Type = DbType.Int64,
                    IsPrimaryKey = true,
                    AutoIncrement = new AutoIncAttribute(10, 3)
                },
            },
        };
    }
}
