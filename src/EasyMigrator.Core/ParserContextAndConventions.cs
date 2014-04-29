using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using EasyMigrator.Model;


namespace EasyMigrator.Parsing
{
    public class Context
    {
        public Conventions Conventions { get; set; }
        public Table Table { get; set; }
        public Type TableType { get; set; }
        public IEnumerable<FieldInfo> Fields { get; set; }
        public object Model { get; set; }
    }

    public class StringLengths
    {
        public int Default { get; set; }
        public int Small { get; set; }
        public int Medium { get; set; }
        public int Large { get; set; }
        public int Max { get; set; }

        public int this[Lengths length]
        {
            get {
                switch (length) {
                    case Lengths.Small: return Small;
                    case Lengths.Medium: return Medium;
                    case Lengths.Large: return Large;
                    case Lengths.Max: return Max;
                    default: return Default;
                }
            }
        }
    }

    public class Conventions
    {
        public StringLengths StringLengths { get; set; }
        public Func<Context, string> TableName { get; set; }
        public Func<Context, Column> PrimaryKey { get; set; }
        public ITypeMap TypeMap { get; set; }

        static public Conventions Default
        {
            get
            {
                return new Conventions {
                    TableName = c => Regex.Replace(c.TableType.Name, "Table$", ""),
                    StringLengths = new StringLengths {
                        Default = 50,
                        Small = 50,
                        Medium = 255,
                        Large = 4000,
                        Max = int.MaxValue
                    },
                    PrimaryKey = c => new Column {
                        Name = "Id",
                        Type = DbType.Int32,
                        AutoIncrement = new AutoIncrementAttribute()
                    },
                    TypeMap = new TypeMap()
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
                            {typeof(decimal), DbType.Currency},
                            {typeof(DateTime), DbType.DateTime},
                            {typeof(Guid), DbType.Guid},
                            {typeof(byte[]), DbType.Binary},
                            {typeof(XDocument), DbType.Xml},
                            {typeof(XmlDocument), DbType.Xml}
                        })
                        .Add(typeof(string),
                                f => f.HasAttribute<AnsiAttribute>()
                                    ? (f.HasAttribute<FixedLengthAttribute>() ? DbType.AnsiStringFixedLength : DbType.AnsiString)
                                    : (f.HasAttribute<FixedLengthAttribute>() ? DbType.StringFixedLength : DbType.String)
                        )
                };
            }
        }
    }
}
