using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using EasyMigrator.Extensions;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Parsing
{
    public partial class Conventions
    {
        static public Conventions Default
            => new Conventions {
                TableName = c => Regex.Replace(c.ModelType.Name, "Table$", ""),
                ColumnName = (c, f) => f.Name,
                PrimaryKey = c => new[] {
                    new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        AutoIncrement = new AutoIncAttribute()
                    }},
                PrimaryKeyName = c => c.Conventions.PrimaryKeyNameByTableName(c.Table.Name),
                PrimaryKeyNameByTableName = t => $"PK_{t}",
                PrimaryKeyColumnName = t => "Id",
                ForeignKeyName = (c, col) => $"FK_{c.Table.Name}_{col.Name}",
                IndexNameByColumns = (c, cols) => c.Conventions.IndexNameByTableAndColumnNames(c.Table.Name, cols.Select(col => col.Name)),
                IndexNameByTableAndColumnNames = (t, cols) => $"IX_{t}_{string.Join("_", cols)}",
                IndexForeignKeys = c => true,
                ColumnLengths = (c, col) => {
                    switch (col.Type) {
                        case DbType.AnsiString:
                        case DbType.AnsiStringFixedLength:
                        case DbType.String:
                        case DbType.StringFixedLength:
                        case DbType.Binary:
                            return new Lengths {
                                Default = 50,
                                Short = 50,
                                Medium = 255,
                                Long = 4000,
                                Max = int.MaxValue
                            };

                        default: return null;
                    }
                },
                PrecisionLengths = (c, col) => {
                    switch (col.Type) {
                        case DbType.Decimal:
                            return new Lengths {
                                Default = 19,
                                Short = 9,
                                Medium = 19,
                                Long = 28,
                                Max = 38
                            };

                        case DbType.DateTime2:
                        case DbType.DateTimeOffset:
                            return new Lengths {
                                Default = 2,
                                Short = 2,
                                Medium = 4,
                                Long = 7,
                                Max = 7
                            };

                        default: return null;
                    }
                },
                ScaleLengths = (c, col) => {
                    switch (col.Type) {
                        case DbType.Decimal:
                            return new Lengths {
                                Default = 9,
                                Short = 2,
                                Medium = 9,
                                Long = 14,
                                Max = 38
                            };

                        default: return null;
                    }
                },
                DefaultPrecision = (c, col) => {
                    var pl = c.Conventions.PrecisionLengths(c, col);
                    var sl = c.Conventions.ScaleLengths(c, col);
                    if (pl == null) return null;
                    switch (col.Type) {
                        case DbType.Decimal:
                        case DbType.DateTime2:
                        case DbType.DateTimeOffset:
                            return sl == null
                                ? new PrecisionAttribute(pl.Default)
                                : new PrecisionAttribute(pl.Default, sl.Default);
                        default: return null;
                    }
                },
                TypeMap = c => new TypeMap()
                    .Add(new Dictionary<Type, DbType> {
                        {typeof(sbyte), DbType.SByte},
                        {typeof(byte), DbType.Byte},
                        {typeof(short), DbType.Int16},
                        {typeof(ushort), DbType.UInt16},
                        {typeof(int), DbType.Int32},
                        {typeof(uint), DbType.UInt32},
                        {typeof(long), DbType.Int64},
                        {typeof(ulong), DbType.UInt64},
                        {typeof(bool), DbType.Boolean},
                        {typeof(float), DbType.Single},
                        {typeof(double), DbType.Double},
                        {typeof(decimal), DbType.Decimal},
                        {typeof(DateTimeOffset), DbType.DateTimeOffset},
                        {typeof(TimeSpan), DbType.Time},
                        {typeof(Guid), DbType.Guid},
                        {typeof(byte[]), DbType.Binary},
                        //{typeof(System.Data.Linq.Binary), DbType.Binary},
                        {typeof(XDocument), DbType.Xml},
                        {typeof(XmlDocument), DbType.Xml},
                    })
                    .Add(new Dictionary<Type, Func<FieldInfo, DbType>> {
                        { typeof(string),
                          f => f.HasAttribute<AnsiAttribute>()
                               ? (f.HasAttribute<FixedAttribute>() ? DbType.AnsiStringFixedLength : DbType.AnsiString)
                               : (f.HasAttribute<FixedAttribute>() ? DbType.StringFixedLength : DbType.String) },
                        { typeof(DateTime),
                          f => f.HasAttribute<PrecisionAttribute>()
                               ? DbType.DateTime2
                               : DbType.DateTime },
                    })
            };
    }
}
