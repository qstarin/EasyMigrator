using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class CustomAutoIncrement_Int32 : TableTestCase
    {
        public class Poco
        {
            [Pk, AutoInc(10, 3)] public int Id;
        }

        static Table Model = new Table {
            Name = "CustomAutoIncrement_Int32",
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
}
