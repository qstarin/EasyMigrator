using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
                ColumnFields = GetColumnFields(type),
                ModelType = type,
                Model = Activator.CreateInstance(type),
            };
            context.Table = new Table { Name = type.GetAttribute<NameAttribute>()?.Name ?? Conventions.TableName(context) };

            return context;
        }

        private readonly ConcurrentDictionary<Type, Context> _parsedTables = new ConcurrentDictionary<Type, Context>();

        public Context ParseTableType(Type type)
            => PostParseTable(_parsedTables.GetOrAdd(type, t => ParseTable(type)));

        protected virtual Context PostParseTable(Context context)
        {
            foreach (var col in context.Table.Columns.Where(c => {
                                                               var f = c.ForeignKey as FkAttribute;
                                                               return f != null && f.Column == null && f.TableType != null;
                                                           }))
            {
                var fk = col.ForeignKey as FkAttribute;
                var fkContext = fk.TableType == context.ModelType ? context : ParseTableType(fk.TableType);
                var fkTable = fkContext.Table;
                if (fkTable.Columns.PrimaryKey().Count() != 1)
                    throw new Exception($"Cannot create a foreign key from table {context.Table.Name} to table {fkTable.Name} with {(fkTable.HasPrimaryKey ? "composite" : "no")} primary key.");

                var fkCol = fkTable.Columns.PrimaryKey().Single();
                var newFk = new FkAttribute(fkTable.Name) {
                    Name = fk.Name,
                    Column = fkCol.Name,
                    Indexed = fk.Indexed,
                };
                col.ForeignKey = newFk;

                if (newFk.Name == null)
                    newFk.Name = Conventions.ForeignKeyName(context, col);
            }
            return context;
        }

        protected virtual Context ParseTable(Type tableType)
        {
            var context = CreateContext(tableType);
            var fields = context.ColumnFields;
            var table = context.Table;
            var typemap = Conventions.TypeMap(context);
            var tablePk = context.ModelType.GetAttribute<PkAttribute>();
            if (tablePk != null) {
                table.PrimaryKeyName = tablePk.Name;
                table.PrimaryKeyIsClustered = tablePk.Clustered;
            }

            foreach (var field in fields) {
                var dbType = field.GetAttribute<DbTypeAttribute>()?.DbType ?? typemap[field];
                var pk = field.GetAttribute<PkAttribute>();
                var fk = field.GetAttribute<FkAttribute>();

                var column = new Column {
                    Name = field.GetAttribute<NameAttribute>()?.Name ?? Conventions.ColumnName(context, field), 
                    Type = dbType,
                    DefaultValue = GetDefaultValue(context.Model, field),
                    IsNullable = IsNullable(field),
                    IsPrimaryKey = pk != null,
                    AutoIncrement = field.GetAttribute<AutoIncAttribute>(),
                    Length = GetLength(field, dbType, Conventions.StringLengths(context)),
                    Index = field.GetAttribute<IndexAttribute>(),
                    ForeignKey = fk,
                    DefinedInPoco = true
                };
                context.Columns.Add(field, column);
                column.Precision = GetPrecision(context, field, column);

                if (pk != null) {
                    if (table.PrimaryKeyName == null)
                        table.PrimaryKeyName = pk.Name;
                    else {
                        if (table.PrimaryKeyName != pk.Name)
                            throw new Exception($"Conflicting primary key names '{table.PrimaryKeyName}' on table {context.Table.Name}, '{pk.Name}' on column {column.Name}");
                    }

                    if (table.PrimaryKeyIsClustered != pk.Clustered) {
                        if (!table.PrimaryKeyIsClustered)
                            throw new Exception($"Conflicting primary key clustering on table {context.Table.Name}");
                        table.PrimaryKeyIsClustered = pk.Clustered;
                    }
                }

                if (column.Index != null && column.Index.Name == null)
                    column.Index.Name = Conventions.IndexNameByColumns(context, new[] { column });

                if (column.ForeignKey != null) {
                    if (column.ForeignKey.Column == null && fk.Table != null)
                        column.ForeignKey.Column = Conventions.PrimaryKeyColumnName(fk.Table);

                    if (column.ForeignKey.Name == null && column.ForeignKey.Column != null)
                        column.ForeignKey.Name = Conventions.ForeignKeyName(context, column);

                    if (column.Index == null && (fk.Indexed || Conventions.IndexForeignKeys(context)))
                        column.Index = new IndexAttribute { Name = Conventions.IndexNameByColumns(context, new[] { column }) };
                }

                if (table.Columns.Any(c => c.Name == column.Name))
                    throw new Exception($"Duplicate column name {column.Name} on table {table.Name}");

                table.Columns.Add(column);
            }

            if (!table.HasPrimaryKey && !context.ModelType.HasAttribute<NoPkAttribute>() && Conventions.PrimaryKey != null) {
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

            var getExpressionFieldGenericMethodInfo = typeof(ReflectionExtensions).GetMethod("GetExpressionField");
            foreach (var fi in GetCompositeIndexFields(tableType)) {
                IndexColumn[] columns = null;

                if (fi.FieldType == typeof(EasyMigrator.CompositeIndex)) {
                    var ci = fi.GetValue(context.Model) as EasyMigrator.CompositeIndex;
                    columns = ci.Columns;
                }
                else {
                    var compositeIndexTableType = fi.FieldType.GetGenericArguments().Single();
                    var compositeIndexParserContext = compositeIndexTableType == tableType ? context : ParseTableType(compositeIndexTableType); // if it is the same type, don't infinitely recurse
                    var compositeIndexInstance = fi.GetValue(context.Model);
                    var compositeIndexColumnsPropertyInfo = fi.FieldType.GetProperty("Columns");
                    var columnsArray = compositeIndexColumnsPropertyInfo.GetGetMethod().Invoke(compositeIndexInstance, null);
                    var compositeIndexConcreteType = compositeIndexColumnsPropertyInfo.PropertyType.GetElementType();
                    var columnExpressionGetMethodInfo = compositeIndexConcreteType.GetProperty("ColumnExpression").GetGetMethod();
                    var directionGetMethodInfo = compositeIndexConcreteType.GetProperty("Direction").GetGetMethod();

                    var columnList = new List<IndexColumn>();
                    foreach (var c in columnsArray as IEnumerable) {
                        var columnExpression = columnExpressionGetMethodInfo.Invoke(c, null);
                        var getExpressionFieldConcreteMethodInfo = getExpressionFieldGenericMethodInfo.MakeGenericMethod(compositeIndexTableType, typeof(object));
                        var columnFieldInfo = getExpressionFieldConcreteMethodInfo.Invoke(null, new[] { columnExpression }) as FieldInfo;
                        var direction = (SortOrder)directionGetMethodInfo.Invoke(c, null);
                        columnList.Add(new IndexColumn(compositeIndexParserContext.Columns[columnFieldInfo].Name, direction));
                    }
                    columns = columnList.ToArray();
                }

                var ciIdxAttr = fi.GetAttribute<IndexAttribute>();
                table.CompositeIndices.Add(new Model.CompositeIndex {
                    Name = ciIdxAttr?.Name ?? Conventions.IndexNameByTableAndColumnNames(table.Name, columns.Select(c => c.ColumnName).ToArray()),
                    Columns = columns,
                    Unique = ciIdxAttr?.Unique ?? false,
                    Clustered = ciIdxAttr?.Clustered ?? false,
                });
            }

            return context;
        }

        protected virtual IEnumerable<FieldInfo> GetColumnFields(Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                   .Where(fi => !IsCompositeIndexField(fi));

        protected virtual IEnumerable<FieldInfo> GetCompositeIndexFields(Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                   .Where(IsCompositeIndexField);

        protected virtual bool IsCompositeIndexField(FieldInfo fi)
            => fi.FieldType == typeof(EasyMigrator.CompositeIndex) ||
               (fi.FieldType.IsGenericType &&
                fi.FieldType.GetGenericTypeDefinition() == typeof(EasyMigrator.CompositeIndex<>));

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

        protected virtual IPrecision GetPrecision(Context context, FieldInfo field, Column column)
        {
            var typesWithPrecision = new[] { DbType.Decimal, DbType.DateTime2, DbType.DateTimeOffset };
            if (!typesWithPrecision.Contains(column.Type))
                return null;

            var precisionAttr = field.GetAttribute<PrecisionAttribute>();
            if (precisionAttr == null)
                return Conventions.DefaultPrecision(context, column);

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
