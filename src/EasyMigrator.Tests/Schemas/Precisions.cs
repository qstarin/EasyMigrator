using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Tests.Schemas
{
    public class Precisions
    {
        public class Unspecified
        {
            public class Poco
            {
                public decimal Rate;
            }

            static Table Model = new Table {
                Name = "Unspecified",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Rate",
                        Type = DbType.Decimal,
                        Precision = new PrecisionAttribute(19, 9),
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(Unspecified))] public class Poco { }
                    static Table Model = Unspecified.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(Unspecified))]
                    public class Poco
                    {
                        public decimal Rate;
                    }
                }
            }
        }

        public class Default
        {
            public class Poco
            {
                [Precision(Length.Default, 2)] public decimal Rate;
                public bool From;
            }

            static Table Model = new Table {
                Name = "Default",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Rate",
                        Type = DbType.Decimal,
                        Precision = new PrecisionAttribute(19, 2),
                    },
                    new Column {
                        Name = "From",
                        Type = DbType.Boolean,
                        DefaultValue = "0",
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(Default))] public class Poco { }
                    static Table Model = Default.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(Default))]
                    public class Poco
                    {
                        [Precision(Length.Default, 2)] public decimal Rate;
                        public bool From;
                    }
                }
            }
        }

        public class Short
        {
            public class Poco
            {
                [Precision(Length.Short, 2)] public decimal Rate;
            }

            static Table Model = new Table {
                Name = "Short",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Rate",
                        Type = DbType.Decimal,
                        Precision = new PrecisionAttribute(9, 2),
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(Short))] public class Poco { }
                    static Table Model = Short.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(Short))]
                    public class Poco
                    {
                        [Precision(Length.Short, 2)] public decimal Rate;
                    }
                }
            }
        }

        public class Medium
        {
            public class Poco
            {
                [Precision(Length.Medium, 2)] public decimal Rate;
            }

            static Table Model = new Table {
                Name = "Medium",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Rate",
                        Type = DbType.Decimal,
                        Precision = new PrecisionAttribute(19, 2),
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(Medium))] public class Poco { }
                    static Table Model = Medium.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(Medium))]
                    public class Poco
                    {
                        [Precision(Length.Medium, 2)] public decimal Rate;
                    }
                }
            }
        }

        public class Long
        {
            public class Poco
            {
                [Precision(Length.Long, 2)] public decimal Rate;
            }

            static Table Model = new Table {
                Name = "Long",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Rate",
                        Type = DbType.Decimal,
                        Precision = new PrecisionAttribute(28, 2),
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(Long))] public class Poco { }
                    static Table Model = Long.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(Long))]
                    public class Poco
                    {
                        [Precision(Length.Long, 2)] public decimal Rate;
                    }
                }
            }
        }

        public class Max
        {
            public class Poco
            {
                [Precision(Length.Max, 2)] public decimal Rate;
            }

            static Table Model = new Table {
                Name = "Max",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Rate",
                        Type = DbType.Decimal,
                        Precision = new PrecisionAttribute(38, 2),
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(Max))] public class Poco { }
                    static Table Model = Max.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(Max))]
                    public class Poco
                    {
                        [Precision(Length.Max, 2)] public decimal Rate;
                    }
                }
            }
        }

        public class Custom_13_9
        {
            public class Poco
            {
                [Precision(13, 9)] public decimal Rate;
                public string From;
            }

            static Table Model = new Table {
                Name = "Custom_13_9",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Rate",
                        Type = DbType.Decimal,
                        Precision = new PrecisionAttribute(13, 9),
                    },
                    new Column {
                        Name = "From",
                        Type = DbType.String,
                        IsNullable = true,
                        Length = 50,
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(Custom_13_9))] public class Poco { }
                    static Table Model = Custom_13_9.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(Custom_13_9))]
                    public class Poco
                    {
                        [Precision(13, 9)] public decimal Rate;
                        public string From;
                    }
                }
            }
        }
    }
}
