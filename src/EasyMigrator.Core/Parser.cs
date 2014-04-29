using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyMigrator.Model;


namespace EasyMigrator.Parsing
{
    public class Parser
    {
        public static Parser Default { get { return _default.Value; } set { _default = new Lazy<Parser>(() => value); } }
        private static Lazy<Parser> _default = new Lazy<Parser>();

        public Conventions Conventions { get; set; }

        public Parser() : this(Conventions.Default) { }
        public Parser(Conventions conventions) { Conventions = conventions; }


        public Table ParseTable(Type type)
        {
            var context = new Context {
                Conventions = Conventions,
                TableType = type,
                Model = Activator.CreateInstance(type)
            };
            var fields = context.Fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            var table = context.Table = new Table {
                Name = Conventions.TableName(context)
            };

            if (!fields.Any(f => f.HasAttribute<PrimaryKeyAttribute>()) && Conventions.PrimaryKey != null) {
                var pk = Conventions.PrimaryKey(context);
                if (fields.Any(f => string.Equals(f.Name, pk.Name, StringComparison.InvariantCultureIgnoreCase)))
                    throw new Exception("The column '" + pk.Name + "' conflicts with the automatically added primary key column name. " +
                                        "Remove the duplicate column definition or add the PrimaryKey attribute to resolve the conflict.");
                table.PrimaryKey.Add(pk);
                table.Columns.Add(pk);
            }

            foreach (var field in fields) {
                var dbType = Conventions.TypeMap[field];
                var col = new Column {
                    Name = field.Name, 
                    Type = dbType,
                    Length = GetLength(field, dbType, Conventions.StringLengths),
                    Precision = field.GetAttribute<PrecisionAttribute>(),
                    AutoIncrement = field.GetAttribute<AutoIncrementAttribute>(),
                    DefaultValue = GetDefaultValue(context.Model, field),
                    IsNullable = IsNullable(field), 
                };
                table.Columns.Add(col);

                if (field.HasAttribute<ForeignKeyAttribute>()) {
                    var fk = field.GetAttribute<ForeignKeyAttribute>();
                    table.ForeignKeys.Add(new ForeignKey {
                        FkTable = table.Name,
                        FkColumn = field.Name,
                        PkTable = fk.Table,
                        PkColumn = fk.Column
                    });

                    if (fk.Indexed && !field.HasAttribute<IndexedAttribute>()) {
                        table.Indexes.Add(new Index {
                            Unique = false,
                            Clustered = false,
                            Table = table.Name,
                            Columns = new[] { field.Name }
                        });
                    }
                }
                
                if (field.HasAttribute<IndexedAttribute>()) {
                    var idx = field.GetAttribute<IndexedAttribute>();
                    table.Indexes.Add(new Index {
                        Unique = idx.Unique,
                        Clustered = idx.cl,
                        Table = table.Name,
                        Columns = new[] { field.Name }
                    });
                }
            }

            return table;
        }

        public static int? GetLength(FieldInfo field, DbType dbType, StringLengths lengths)
        {
            var typesWithLength = new[] {
                DbType.AnsiString,
                DbType.AnsiStringFixedLength,
                DbType.String,
                DbType.StringFixedLength,
                DbType.Binary
            };

            if (!typesWithLength.Contains(dbType))
                return null;

            var lengthAttr = field.GetAttribute<LengthAttribute>();
            if (lengthAttr == null)
                return lengths.Default;

            if (lengthAttr.LengthEnum.HasValue)
                return lengths[lengthAttr.LengthEnum.Value];

            return lengthAttr.Length;
        }

        public static bool IsNullable(FieldInfo field)
        {
            var nullableAttr = field.GetAttribute<NullableAttribute>();
            return nullableAttr != null
                       ? nullableAttr.Nullable
                       : field.FieldType.IsNullableType() || !field.FieldType.IsValueType;
        }

        private static string GetDefaultValue(object model, FieldInfo field)
        {
            if (field.HasAttribute<DefaultAttribute>())
                return field.GetAttribute<DefaultAttribute>().Expression;

            var val = field.GetValue(model);
            if (field.FieldType == typeof(bool))
                return (bool)val ? "1" : "0"; // special case - always set a default for bools
            if (val == null || val.Equals(Activator.CreateInstance(field.FieldType)))
                return null;
            else if (field.FieldType.IsNumeric())
                return val.ToString();
            else
                return "'" + val.ToString().Replace("'", "''") + "'";
        }
    }
}
