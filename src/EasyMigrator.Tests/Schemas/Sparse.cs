using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Schemas
{
    public class Sparse : TableTestCase
    {
        public class SparseData
        {
            public class Poco
            {
                [Pk] public long Id;
                [Sparse] public int? RT000;
                [Sparse] public int? RT001;
                [Sparse] public int? RT002;
                [Sparse] public int? RT003;
            }


            static public Table Model = new Table {
                Name = "SparseData",
                Columns = new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int64,
                        IsPrimaryKey = true,
                    },
                    new Column {
                        Name = "RT000",
                        Type = DbType.Int32,
                        IsNullable = true,
                        IsSparse = true,
                    },
                    new Column {
                        Name = "RT001",
                        Type = DbType.Int32,
                        IsNullable = true,
                        IsSparse = true,
                    },
                    new Column {
                        Name = "RT002",
                        Type = DbType.Int32,
                        IsNullable = true,
                        IsSparse = true,
                    },
                    new Column {
                        Name = "RT003",
                        Type = DbType.Int32,
                        IsNullable = true,
                        IsSparse = true,
                    },
                }
            };
        }
    }
}
