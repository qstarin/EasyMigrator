using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Tests.Schemas
{
    public class AddColumns_WithPopulate_IntAndString
    {
        public class AddColumns
        {
            [NPoco.TableName("AddColumns")]
            public class Poco
            {
                public string Name;
            }

            [Name("AddColumns")]
            public class ColumnsToAdd
            {
                public int Quantity;
                [Max, NotNull] public string Story;
            }

            static Table Model = new Table {
                Name = "AddColumns",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Name",
                        Type = DbType.String,
                        Length = 50,
                        IsNullable = true,
                    },
                    new Column {
                        Name = "Quantity",
                        Type = DbType.Int32,
                    },
                    new Column {
                        Name = "Story",
                        Type = DbType.String,
                        Length = int.MaxValue,
                    },
                }
            };
        }
    }
}
