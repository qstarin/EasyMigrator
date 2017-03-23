using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Tests.Schemas
{
    public class DateTime2
    {
        public class WithScale
        {
            public class Poco
            {
                [DbType(DbType.DateTime2), Precision(2)] public DateTime CreatedOn;
            }

            static Table Model = new Table {
                Name = "WithScale",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "CreatedOn",
                        Type = DbType.DateTime2,
                        Precision = new PrecisionAttribute(2),
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(WithScale))] public class Poco { }
                    static Table Model = WithScale.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(WithScale))]
                    public class Poco
                    {
                        [DbType(DbType.DateTime2), Precision(2)] public DateTime CreatedOn;
                    }
                }
            }
        }
        public class NotNull
        {
            public class Poco
            {
                [DbType(DbType.DateTime2)] public DateTime CreatedOn;
            }

            static Table Model = new Table {
                Name = "NotNull",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "CreatedOn",
                        Type = DbType.DateTime2,
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(NotNull))] public class Poco { }
                    static Table Model = NotNull.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(NotNull))]
                    public class Poco
                    {
                        [DbType(DbType.DateTime2)] public DateTime CreatedOn;
                    }
                }
            }
        }
    }
}
