using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<Type, Table> _parsedTables = new ConcurrentDictionary<Type, Table>();

        public Table ParseTableType(Type type)
        {
            var context = CreateContext(type);
            context.Table = _parsedTables.GetOrAdd(type, t => ParseTable(context));
            PostParseTable(context);
            return context.Table;
        }

        protected virtual void PostParseTable(Context context)
        {
            foreach (var col in context.Table.Columns.Where(c => {
                                                               var f = c.ForeignKey as FkAttribute;
                                                               return f != null && f.Column == null && f.TableType != null;
                                                           }))
            {
                var fk = col.ForeignKey as FkAttribute;
                var fkTable = ParseTableType(fk.TableType);
                if (fkTable.PrimaryKeyColumns.Count() != 1)
                    throw new Exception($"Cannot create a foreign key from table {context.Table.Name} to table {fkTable.Name} with {(fkTable.HasPrimaryKey ? "composite" : "no")} primary key.");

                var fkCol = fkTable.PrimaryKeyColumns.Single();
                fk.Column = fkCol.Name;
                if (fk.Name == null)
                    fk.Name = Conventions.ForeignKeyName(context, col);
            }
        }

        protected virtual Table ParseTable(Context context)
        {
            var fields = context.Fields;
            var table = context.Table;
            var typemap = Conventions.TypeMap(context);
            table.PrimaryKeyName = context.ModelType.GetAttribute<PkAttribute>()?.Name;

            foreach (var field in fields) {
                var dbType = typemap[field];
                var pk = field.GetAttribute<PkAttribute>();
                var fk = field.GetAttribute<FkAttribute>();

                var column = new Column {
                    Name = Conventions.ColumnName(context, field), 
                    Type = dbType,
                    DefaultValue = GetDefaultValue(context.Model, field),
                    IsNullable = IsNullable(field),
                    IsPrimaryKey = pk != null,
                    AutoIncrement = field.GetAttribute<AutoIncAttribute>(),
                    Length = GetLength(field, dbType, Conventions.StringLengths(context)),
                    Precision = GetPrecision(context, field, dbType),
                    Index = field.GetAttribute<IndexAttribute>(),
                    ForeignKey = fk,
                    DefinedInPoco = true
                };

                if (pk != null) {
                    if (table.PrimaryKeyName == null)
                        table.PrimaryKeyName = pk.Name;
                    else {
                        if (table.PrimaryKeyName != pk.Name)
                            throw new Exception($"Conflicting primary key names '{table.PrimaryKeyName}' on table {context.Table.Name}, '{pk.Name}' on column {column.Name}");
                    }
                }

                if (column.Index != null && column.Index.Name == null)
                    column.Index.Name = Conventions.IndexName(context, new[] { column });

                if (column.ForeignKey != null) {
                    if (column.ForeignKey.Column == null && fk.Table != null)
                        column.ForeignKey.Column = Conventions.PrimaryKeyColumnName(fk.Table);

                    if (column.ForeignKey.Name == null && column.ForeignKey.Column != null)
                        column.ForeignKey.Name = Conventions.ForeignKeyName(context, column);

                    if (column.Index == null && (fk.Indexed == true || Conventions.IndexForeignKeys(context)))
                        column.Index = new IndexAttribute { Name = Conventions.IndexName(context, new[] { column }) };
                }

                if (table.Columns.Any(c => c.Name == column.Name))
                    throw new Exception($"Duplicate column name {column.Name} on table {table.Name}");

                table.Columns.Add(column);
            }

            if (!table.HasPrimaryKey && !context.ModelType.HasAttribute<NoPkAttribute>()) {
                foreach (var pk in Conventions.PrimaryKey(context).Reverse()) { // Reverse so Insert(0.. puts everything in the right order
                    pk.IsPrimaryKey = true;
                    pk.DefinedInPoco = false;
                    if (table.Columns.Any(c => c.Name == pk.Name))
                        throw new Exception($"The column {pk.Name} conflicts with the automatically added primary key column name on table {table.Name}. " +
                                            "Remove the duplicate column definition or add the PrimaryKey attribute to resolve the conflict.");
                    table.Columns.Insert(0, pk);
                }
            }

            if (table.PrimaryKeyName == null)
                table.PrimaryKeyName = Conventions.PrimaryKeyName(context);

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
                return new PrecisionAttribute(Conventions.PrecisionLengths(context)[precisionAttr.DefinedPrecision.Value], precisionAttr.Scale);

            return precisionAttr;
        }

        protected virtual bool IsNullable(FieldInfo field)
#pragma warning disable 618
            => (field.FieldType.IsNullableType() || !field.FieldType.IsValueType || field.HasAttribute<NullAttribute>()) && !field.HasAttribute<NotNullAttribute>();
#pragma warning restore 618
    }
}
