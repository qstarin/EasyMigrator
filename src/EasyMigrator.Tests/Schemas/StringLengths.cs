using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Tests.Schemas
{
    public class StringLengths
    {
        public class Unspecified
        {
            public class Poco
            {
                public string Name;
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
                        Name = "Name",
                        Type = DbType.String,
                        Length = 50,
                        IsNullable = true,
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
                        public string Name;
                    }
                }
            }
        }

        public class Default
        {
            public class Poco
            {
                [Length(Length.Default)] public string Name;
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
                        Name = "Name",
                        Type = DbType.String,
                        Length = 50,
                        IsNullable = true,
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
                        [Length(Length.Default)] public string Name;
                    }
                }
            }
        }

        public class Short
        {
            public class Poco
            {
                [Length(Length.Short)] public string Name;
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
                        Name = "Name",
                        Type = DbType.String,
                        Length = 50,
                        IsNullable = true,
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
                        [Length(Length.Short)] public string Name;
                    }
                }
            }
        }

        public class Medium
        {
            public class Poco
            {
                [Length(Length.Medium)] public string Name;
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
                        Name = "Name",
                        Type = DbType.String,
                        Length = 255,
                        IsNullable = true,
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
                        [Length(Length.Medium)] public string Name;
                    }
                }
            }
        }

        public class Long
        {
            public class Poco
            {
                [Length(Length.Long)] public string Name;
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
                        Name = "Name",
                        Type = DbType.String,
                        Length = 4000,
                        IsNullable = true,
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
                        [Length(Length.Long)] public string Name;
                    }
                }
            }
        }

        public class Max
        {
            public class Poco
            {
                [Length(Length.Max)] public string Name;
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
                        Name = "Name",
                        Type = DbType.String,
                        Length = int.MaxValue,
                        IsNullable = true,
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
                        [Length(Length.Max)] public string Name;
                    }
                }
            }
        }

        public class Custom_500
        {
            public class Poco
            {
                [Length(500)] public string Name;
            }

            static Table Model = new Table {
                Name = "Custom_500",
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
                        Length = 500,
                        IsNullable = true,
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(Custom_500))] public class Poco { }
                    static Table Model = Custom_500.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(Custom_500))]
                    public class Poco
                    {
                        [Length(500)] public string Name;
                    }
                }
            }
        }

        public class Fixed_20
        {
            public class Poco
            {
                [Fixed(20)] public string Name;
            }

            static Table Model = new Table {
                Name = "Fixed_20",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        IsPrimaryKey = true,
                        AutoIncrement = new AutoIncAttribute()
                    },
                    new Column {
                        Name = "Name",
                        Type = DbType.StringFixedLength,
                        Length = 20,
                        IsNullable = true,
                    },
                }
            };

            public class AddColumns
            {
                public class Empty
                {
                    [Name(nameof(Fixed_20))] public class Poco { }
                    static Table Model = Fixed_20.Model;
                }

                public class ColumnsToAdd
                {
                    [Name(nameof(Fixed_20))]
                    public class Poco
                    {
                        [Fixed(20)] public string Name;
                    }
                }
            }
        }

        public class Ansi
        {
            public class Unspecified
            {
                public class Poco
                {
                    [Ansi] public string Name;
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
                            Name = "Name",
                            Type = DbType.AnsiString,
                            Length = 50,
                            IsNullable = true,
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
                            [Ansi] public string Name;
                        }
                    }
                }
            }

            public class Default
            {
                public class Poco
                {
                    [Length(Length.Default), Ansi] public string Name;
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
                            Name = "Name",
                            Type = DbType.AnsiString,
                            Length = 50,
                            IsNullable = true,
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
                            [Length(Length.Default), Ansi] public string Name;
                        }
                    }
                }
            }

            public class Short
            {
                public class Poco
                {
                    [Length(Length.Short), Ansi] public string Name;
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
                            Name = "Name",
                            Type = DbType.AnsiString,
                            Length = 50,
                            IsNullable = true,
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
                            [Length(Length.Short), Ansi] public string Name;
                        }
                    }
                }
            }

            public class Medium
            {
                public class Poco
                {
                    [Length(Length.Medium), Ansi] public string Name;
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
                            Name = "Name",
                            Type = DbType.AnsiString,
                            Length = 255,
                            IsNullable = true,
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
                            [Length(Length.Medium), Ansi] public string Name;
                        }
                    }
                }
            }

            public class Long
            {
                public class Poco
                {
                    [Length(Length.Long), Ansi] public string Name;
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
                            Name = "Name",
                            Type = DbType.AnsiString,
                            Length = 4000,
                            IsNullable = true,
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
                            [Length(Length.Long), Ansi] public string Name;
                        }
                    }
                }
            }

            public class Max
            {
                public class Poco
                {
                    [Length(Length.Max), Ansi] public string Name;
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
                            Name = "Name",
                            Type = DbType.AnsiString,
                            Length = int.MaxValue,
                            IsNullable = true,
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
                            [Length(Length.Max), Ansi] public string Name;
                        }
                    }
                }
            }

            public class Custom_500
            {
                public class Poco
                {
                    [Length(500), Ansi] public string Name;
                }

                static Table Model = new Table {
                    Name = "Custom_500",
                    Columns = new[] {
                        new Column {
                            Name = "Id",
                            Type = DbType.Int32,
                            IsPrimaryKey = true,
                            AutoIncrement = new AutoIncAttribute()
                        },
                        new Column {
                            Name = "Name",
                            Type = DbType.AnsiString,
                            Length = 500,
                            IsNullable = true,
                        },
                    }
                };

                public class AddColumns
                {
                    public class Empty
                    {
                        [Name(nameof(Custom_500))] public class Poco { }
                        static Table Model = Custom_500.Model;
                    }

                    public class ColumnsToAdd
                    {
                        [Name(nameof(Custom_500))]
                        public class Poco
                        {
                            [Length(500), Ansi] public string Name;
                        }
                    }
                }
            }

            public class Fixed_20
            {
                public class Poco
                {
                    [Fixed(20), Ansi] public string Name;
                }

                static Table Model = new Table {
                    Name = "Fixed_20",
                    Columns = new[] {
                        new Column {
                            Name = "Id",
                            Type = DbType.Int32,
                            IsPrimaryKey = true,
                            AutoIncrement = new AutoIncAttribute()
                        },
                        new Column {
                            Name = "Name",
                            Type = DbType.AnsiStringFixedLength,
                            Length = 20,
                            IsNullable = true,
                        },
                    }
                };

                public class AddColumns
                {
                    public class Empty
                    {
                        [Name(nameof(Fixed_20))] public class Poco { }
                        static Table Model = Fixed_20.Model;
                    }

                    public class ColumnsToAdd
                    {
                        [Name(nameof(Fixed_20))]
                        public class Poco
                        {
                            [Fixed(20), Ansi] public string Name;
                        }
                    }
                }
            }
        }
    }
}
