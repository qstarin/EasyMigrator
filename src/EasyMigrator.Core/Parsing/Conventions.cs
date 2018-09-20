using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Parsing
{
    public partial class Conventions
    {
        public Func<Context, string> TableName { get; set; }
        public Func<Context, FieldInfo, string> ColumnName { get; set; }
        public Func<Context, IEnumerable<Column>> PrimaryKey { get; set; }
        public Func<Context, string> PrimaryKeyName { get; set; }
        public Func<string, string> PrimaryKeyNameByTableName { get; set; }
        public Func<string, string> PrimaryKeyColumnName { get; set; }
        public Func<Context, Column, string> ForeignKeyName { get; set; }
        public Func<Context, IEnumerable<Column>, string> IndexNameByColumns { get; set; }
        public Func<string, IEnumerable<string>, string> IndexNameByTableAndColumnNames { get; set; }
        public Func<Context, ITypeMap> TypeMap { get; set; }
        public Func<Context, Column, Lengths> ColumnLengths { get; set; }
        public Func<Context, Column, Lengths> PrecisionLengths { get; set; }
        public Func<Context, Column, Lengths> ScaleLengths { get; set; }
        public Func<Context, Column, IPrecision> DefaultPrecision { get; set; }
        public Func<Context, bool> IndexForeignKeys { get; set; }

        public Conventions Clone()
            => new Conventions {
                TableName = TableName,
                ColumnName = ColumnName,
                PrimaryKey = PrimaryKey,
                PrimaryKeyName = PrimaryKeyName,
                PrimaryKeyNameByTableName = PrimaryKeyNameByTableName,
                PrimaryKeyColumnName = PrimaryKeyColumnName,
                ForeignKeyName = ForeignKeyName,
                IndexNameByColumns = IndexNameByColumns,
                IndexNameByTableAndColumnNames = IndexNameByTableAndColumnNames,
                TypeMap = TypeMap,
                ColumnLengths = ColumnLengths,
                PrecisionLengths = PrecisionLengths,
                ScaleLengths = ScaleLengths,
                DefaultPrecision = DefaultPrecision,
                IndexForeignKeys = IndexForeignKeys,
            };
    }
}
