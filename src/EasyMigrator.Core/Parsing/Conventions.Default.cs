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
                PrimaryKeyNameByTableName = s => $"PK_{s}",
                PrimaryKeyColumnName = t => "Id",
                ForeignKeyName = (c, col) => $"FK_{col.ForeignKey.Table}_{col.Name}",
                IndexNameByColumns = (c, cols) => c.Conventions.IndexNameByTableAndColumnNames(c.Table.Name, cols.Select(col => col.Name)),
                IndexNameByTableAndColumnNames = (t, cols) => $"IX_{t}_{string.Join("_", cols)}",
                IndexForeignKeys = c => true,
                StringLengths = c => new Lengths {
                    Default = 50,
                    Short = 50,
                    Medium = 255,
                    Long = 4000,
                    Max = int.MaxValue
                },
                PrecisionLengths = c => new Lengths {
                    Default = 19,
                    Short = 9,
                    Medium = 19,
                    Long = 28,
                    Max = 38
                },
                DefaultPrecision = c => new PrecisionAttribute(c.Conventions.PrecisionLengths(c).Default, 2),
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
                        {typeof(DateTime), DbType.DateTime},
                        {typeof(DateTimeOffset), DbType.DateTimeOffset},
                        {typeof(TimeSpan), DbType.Time},
                        {typeof(Guid), DbType.Guid},
                        {typeof(byte[]), DbType.Binary},
                        //{typeof(System.Data.Linq.Binary), DbType.Binary},
                        {typeof(XDocument), DbType.Xml},
                        {typeof(XmlDocument), DbType.Xml}
                    })
                    .Add(typeof(string),
                            f => f.HasAttribute<AnsiAttribute>()
                                ? (f.HasAttribute<FixedAttribute>() ? DbType.AnsiStringFixedLength : DbType.AnsiString)
                                : (f.HasAttribute<FixedAttribute>() ? DbType.StringFixedLength : DbType.String)
                    )
            };
    }
}
