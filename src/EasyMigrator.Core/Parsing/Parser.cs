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
    public interface IParser
    {
        Conventions Conventions { get; set; }
        Context ParseTableType(Type type);
    }

    public class Parser : IParser
    {
        public Parser() : this(Conventions.Default) { }
        public Parser(Conventions conventions) { Conventions = conventions; }


        #region Current

        static public IParser Current { get; set; } = new Parser();
        static public IDisposable Override(IParser newParser) => new ParserScope(newParser);
        private class ParserScope : IDisposable
        {
            private readonly IParser _previousParser;
            public ParserScope(IParser newParser)
            {
                _previousParser = Parser.Current;
                Parser.Current = newParser;
            }
            public void Dispose() { Parser.Current = _previousParser; }
        }

        #endregion


        #region Conventions

        public Conventions Conventions { get; set; }
        static public IDisposable Override(Conventions newConventions) => new ConventionsScope(newConventions);
        static public IDisposable Override(Action<Conventions> newConventions)
        {
            var c = Parser.Current.Conventions.Clone();
            newConventions(c);
            return new ConventionsScope(c);
        }
        private class ConventionsScope : IDisposable
        {
            private readonly Conventions _previousConventions;
            public ConventionsScope(Conventions newConventions)
            {
                _previousConventions = Parser.Current.Conventions;
                Parser.Current.Conventions = newConventions;
            }
            public void Dispose() { Parser.Current.Conventions = _previousConventions; }
        }

        #endregion


        #region Create Context

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

        #endregion


        #region Parse Table

        private readonly ConcurrentDictionary<Type, Context> _parsedTables = new ConcurrentDictionary<Type, Context>();
        public Context ParseTableType(Type type) => PostParseTable(_parsedTables.GetOrAdd(type, t => ParseTable(type)));

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
                var typeAttr = field.GetAttribute<DbTypeAttribute>();
                var dbType = typeAttr?.DbType ?? typemap[field];
                var pk = field.GetAttribute<PkAttribute>();
                var fk = field.GetAttribute<FkAttribute>();
                var idx = field.GetAttribute<IndexAttribute>();

                var column = new Column {
                    Name = field.GetAttribute<NameAttribute>()?.Name ?? Conventions.ColumnName(context, field), 
                    Type = dbType,
                    CustomType = typeAttr?.CustomType,
                    DefaultValue = GetDefaultValue(context.Model, field),
                    IsFixedLength = field.HasAttribute<FixedAttribute>(),
                    IsNullable = IsNullable(field),
                    IsPrimaryKey = pk != null,
                    IsSparse = field.HasAttribute<SparseAttribute>(),
                    AutoIncrement = field.GetAttribute<AutoIncAttribute>(),
                    ForeignKey = fk,
                    DefinedInPoco = true
                };
                context.Columns.Add(field, column);

                column.Length = GetLength(context, field, column);
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


                if (column.ForeignKey != null) {
                    if (column.ForeignKey.Column == null && fk.Table != null)
                        column.ForeignKey.Column = Conventions.PrimaryKeyColumnName(fk.Table);

                    if (column.ForeignKey.Name == null && column.ForeignKey.Column != null)
                        column.ForeignKey.Name = Conventions.ForeignKeyName(context, column);

                    if (idx == null && (fk.Indexed || (!fk.IndexedWasSet && Conventions.IndexForeignKeys(context))))
                        idx = new IndexAttribute();
                }

                if (idx != null) {
                    table.Indices.Add(new Index(new[] { new IndexColumn(column.Name) }) {
                        Name = idx.Name ?? Conventions.IndexNameByColumns(context, new[] { column }),
                        Clustered = idx.Clustered,
                        Unique = idx.Unique,
                        Where = idx.Where,
                        With = idx.With,
                    });
                }

                if (table.Columns.Any(c => c.Name == column.Name))
                    throw new Exception($"Duplicate column name {column.Name} on table {table.Name}");

                table.Columns.Add(column);
            }


            if (!table.HasPrimaryKey && !context.ModelType.HasAttribute<NoPkAttribute>() && Conventions.PrimaryKey != null) {
                foreach (var pk in Conventions.PrimaryKey(context).Reverse()) { // Reverse so Insert(0.. puts everything in the right order (placing PK columns first)
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

            foreach (var fi in GetCompositeIndexFields(tableType))
                table.Indices.Add(BuildIndex(context, table, fi));

            return context;
        }

        static private readonly MethodInfo _getExprField = typeof(ReflectionExtensions).GetMethod("GetExpressionField");
        static private readonly MethodInfo _getName = typeof(Index).GetProperty("Name").GetGetMethod();
        static private readonly MethodInfo _getClustered = typeof(Index).GetProperty("Clustered").GetGetMethod();
        static private readonly MethodInfo _getUnique = typeof(Index).GetProperty("Unique").GetGetMethod();
        static private readonly MethodInfo _getWhere = typeof(Index).GetProperty("Where").GetGetMethod();
        static private readonly MethodInfo _getWith = typeof(Index).GetProperty("With").GetGetMethod();
        private IIndex BuildIndex(Context context, Table table, FieldInfo fi)
        {
            IndexColumn[] columns = null;
            IndexColumn[] includes = null;

            var fv = fi.GetValue(context.Model);
            var getName = _getName;
            var getClustered = _getClustered;
            var getUnique = _getUnique;
            var getWhere = _getWhere;
            var getWith = _getWith;


            if (fv is Index ci) {
                columns = ci.Columns;
                includes = ci.Includes;
            }
            else {
                var idxTableType = fi.FieldType.GetGenericArguments().Single();
                var idxContext = idxTableType == context.ModelType ? context : ParseTableType(idxTableType); // if it is the same type, don't infinitely recurse
                
                var columnsProp = fi.FieldType.GetProperty("Columns");
                var columnsArray = columnsProp.GetGetMethod().Invoke(fv, null);

                var colType = columnsProp.PropertyType.GetElementType();
                var getColExpr = colType.GetProperty("ColumnExpression").GetGetMethod();
                var getDirection = colType.GetProperty("Direction").GetGetMethod();

                var includesProp = fi.FieldType.GetProperty("Includes");
                var includesArray = includesProp.GetGetMethod().Invoke(fv, null);

                var getExprField = _getExprField.MakeGenericMethod(idxTableType, typeof(object));
                getName = fi.FieldType.GetProperty("Name").GetGetMethod();
                getClustered = fi.FieldType.GetProperty("Clustered").GetGetMethod();
                getUnique = fi.FieldType.GetProperty("Unique").GetGetMethod();
                getWhere = fi.FieldType.GetProperty("Where").GetGetMethod();
                getWith = fi.FieldType.GetProperty("With").GetGetMethod();

                var columnList = new List<IndexColumn>();
                foreach (var c in columnsArray as IEnumerable) {
                    var columnExpression = getColExpr.Invoke(c, null);
                    var columnFieldInfo = getExprField.Invoke(null, new[] { columnExpression }) as FieldInfo;
                    var direction = (SortOrder)getDirection.Invoke(c, null);
                    columnList.Add(new IndexColumn(idxContext.Columns[columnFieldInfo].Name, direction));
                }
                columns = columnList.ToArray();

                var includeList = new List<IndexColumn>();
                foreach (var c in includesArray as IEnumerable) {
                    var columnExpression = getColExpr.Invoke(c, null);
                    var columnFieldInfo = getExprField.Invoke(null, new[] { columnExpression }) as FieldInfo;
                    var direction = (SortOrder)getDirection.Invoke(c, null);
                    includeList.Add(new IndexColumn(idxContext.Columns[columnFieldInfo].Name, direction));
                }
                includes = includeList.ToArray();
            }

            var attr = fi.GetAttribute<IndexAttribute>();
            return new Index(columns, includes) {
                Name = attr?.Name ?? (string)getName.Invoke(fv, null) ?? Conventions.IndexNameByTableAndColumnNames(table.Name, columns.Select(c => c.ColumnName).ToArray()),
                Unique = attr?.Unique ?? (bool)getUnique.Invoke(fv, null),
                Clustered = attr?.Clustered ?? (bool)getClustered.Invoke(fv, null),
                Where = attr?.Where ?? (string)getWhere.Invoke(fv, null),
                With = attr?.With ?? (string)getWith.Invoke(fv, null),
            };
        }

        protected virtual Context PostParseTable(Context context)
        {
            // second pass to create foreign keys from attributes using table type instead of strings
            foreach (var col in context.Table.Columns.Where(c => c.ForeignKey is FkAttribute fk && fk.Column == null && fk.TableType != null))
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

                var ifk = (IForeignKey)fk;
                if (ifk.OnDelete.HasValue)
                    newFk.OnDelete = ifk.OnDelete.Value;
                if (ifk.OnUpdate.HasValue)
                    newFk.OnUpdate = ifk.OnUpdate.Value;

                col.ForeignKey = newFk;

                if (newFk.Name == null)
                    newFk.Name = Conventions.ForeignKeyName(context, col);
            }
            return context;
        }

        #endregion


        #region Helpers

        protected virtual IEnumerable<FieldInfo> GetColumnFields(Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                   .Where(fi => !IsCompositeIndexField(fi));

        protected virtual IEnumerable<FieldInfo> GetCompositeIndexFields(Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                   .Where(IsCompositeIndexField);

        protected virtual bool IsCompositeIndexField(FieldInfo fi)
            => fi.FieldType == typeof(EasyMigrator.Index) ||
              (fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(EasyMigrator.Index<>));

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

        protected virtual int? GetLength(Context context, FieldInfo field, Column column)
        {
            var lengths = Conventions.ColumnLengths(context, column);

            var lengthAttr = field.GetAttribute<LengthAttribute>();
            if (lengthAttr == null)
                return lengths?.Default;

            if (lengthAttr.DefinedLength.HasValue)
                return lengths[lengthAttr.DefinedLength.Value];

            return lengthAttr.Length;
        }

        protected virtual IPrecision GetPrecision(Context context, FieldInfo field, Column column)
        {
            var precisionAttr = field.GetAttribute<PrecisionAttribute>();
            if (precisionAttr == null)
                return Conventions.DefaultPrecision(context, column);

            var p = precisionAttr.DefinedPrecision.HasValue
                ? Conventions.PrecisionLengths(context, column)[precisionAttr.DefinedPrecision.Value]
                : precisionAttr.Precision;

            var s = precisionAttr.DefinedScale.HasValue
                ? Conventions.ScaleLengths(context, column)[precisionAttr.DefinedScale.Value]
                : precisionAttr.Scale;

            return new PrecisionAttribute(p, s);
        }

        protected virtual bool IsNullable(FieldInfo field)
#pragma warning disable 618
            => (field.FieldType.IsNullableType() || !field.FieldType.IsValueType || field.HasAttribute<NullAttribute>()) && !field.HasAttribute<NotNullAttribute>();
#pragma warning restore 618

        #endregion

    }
}
