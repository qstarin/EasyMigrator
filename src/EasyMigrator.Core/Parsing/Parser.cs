using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyMigrator.Extensions;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Parsing
{
    public class Parser
    {
        static public Parser Default { get; set; } = new Parser();
        public Conventions Conventions { get; set; }

        public Parser() : this(Conventions.Default) { }
        public Parser(Conventions conventions) { Conventions = conventions; }

        protected virtual Context CreateContext(Type type)
        {
            var context = new Context {
                Conventions = Conventions,
                Fields = GetColumnFields(type),
                ModelType = type,
                Model = Activator.CreateInstance(type),
            };
            context.Table = new Table { Name = Conventions.TableName(context) };

            return context;
        }

        public Table ParseTableType(Type type) => ParseTable(CreateContext(type));

        protected virtual Table ParseTable(Context context)
        {
            var fields = context.Fields;
            var table = context.Table;

            if (!fields.Any(f => f.HasAttribute<PkAttribute>()) && Conventions.PrimaryKey != null) {
                var pk = Conventions.PrimaryKey(context);
                pk.IsPrimaryKey = true;
                pk.DefinedInPoco = false;
                if (fields.Any(f => string.Equals(f.Name, pk.Name, StringComparison.InvariantCultureIgnoreCase)))
                    throw new Exception("The column '" + pk.Name + "' conflicts with the automatically added primary key column name. " +
                                        "Remove the duplicate column definition or add the PrimaryKey attribute to resolve the conflict.");
                table.Columns.Add(pk);
            }

            foreach (var field in fields) {
                var dbType = Conventions.TypeMap[field];

                var column = new Column {
                    Name = field.Name, 
                    Type = dbType,
                    DefaultValue = GetDefaultValue(context.Model, field),
                    IsNullable = IsNullable(field),
                    IsPrimaryKey = field.HasAttribute<PkAttribute>(),
                    AutoIncrement = field.GetAttribute<AutoIncAttribute>(),
                    Length = GetLength(field, dbType, Conventions.StringLengths),
                    Precision = GetPrecision(context, field, dbType),
                    Index = field.GetAttribute<IndexAttribute>(),
                    ForeignKey = field.GetAttribute<FkAttribute>(),
                    DefinedInPoco = true
                };

                if (column.ForeignKey != null && column.Index == null && Conventions.IndexForeignKeys)
                    column.Index = new IndexAttribute();

                table.Columns.Add(column);
            }

            return table;
        }

        protected virtual IEnumerable<FieldInfo> GetColumnFields(Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        protected virtual string GetDefaultValue(object model, FieldInfo field)
        {
            if (field.HasAttribute<DefaultAttribute>())
                return field.GetAttribute<DefaultAttribute>().Expression;

            var val = field.GetValue(model);
            if (field.FieldType == typeof(bool))
                return val != null && (bool)val ? "1" : "0"; // special case - always set a default for bools
            else if (val == null || (field.FieldType.IsValueType && val.Equals(Activator.CreateInstance(field.FieldType))))
                return null;
            else
                return val.ToString();
        }

        protected virtual int? GetLength(FieldInfo field, DbType dbType, Lengths lengths)
        {
            // TODO: Possibly double lengths for ansi? -> do it by making Lengths key off of DbType?
            var typesWithLength = new[] {
                DbType.AnsiString,
                DbType.AnsiStringFixedLength,
                DbType.String,
                DbType.StringFixedLength,
                DbType.Binary // TODO: What about varbinary?
            };

            if (!typesWithLength.Contains(dbType))
                return null;

            var lengthAttr = field.GetAttribute<LengthAttribute>();
            if (lengthAttr == null)
                return lengths.Default;

            if (lengthAttr.DefinedLength.HasValue)
                return lengths[lengthAttr.DefinedLength.Value];

            return lengthAttr.Length;
        }

        protected virtual IPrecision GetPrecision(Context context, FieldInfo field, DbType dbType)
        {
            var typesWithPrecision = new[] { DbType.Decimal };
            if (!typesWithPrecision.Contains(dbType))
                return null;

            var precisionAttr = field.GetAttribute<PrecisionAttribute>();
            if (precisionAttr == null)
                return Conventions.DefaultPrecision(context);

            if (precisionAttr.DefinedPrecision.HasValue)
                return new PrecisionAttribute(Conventions.PrecisionLengths[precisionAttr.DefinedPrecision.Value], precisionAttr.Scale);

            return precisionAttr;
        }

        protected virtual bool IsNullable(FieldInfo field)
            => (field.FieldType.IsNullableType() || !field.FieldType.IsValueType) && !field.HasAttribute<NotNullAttribute>();
    }
}
