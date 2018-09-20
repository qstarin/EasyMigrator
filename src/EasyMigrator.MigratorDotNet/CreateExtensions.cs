using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Extensions;
using Migrator.Framework;
using EColumn = EasyMigrator.Parsing.Model.Column;

namespace EasyMigrator
{
    static public class CreateExtensions
    {
        static public void AddTable<T>(this ITransformationProvider Database) => Database.AddTable(typeof(T));
        static public void AddTable(this ITransformationProvider Database, Type tableType)
        {
            var table = tableType.ParseTable().Table;
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ");
            sb.Append(table.Name.SqlQuote());
            sb.AppendLine(" (");

            for (var i = 0; i < table.Columns.Count; i++) {
                BuildFullColumnSpec(sb, table, table.Columns[i]);
                if (i < table.Columns.Count - 1)
                    sb.Append(',');
                sb.AppendLine();
            }

            if (table.HasPrimaryKey) {
                sb.Append("CONSTRAINT ");
                sb.Append(table.PrimaryKeyName.SqlQuote());
                sb.Append(" PRIMARY KEY ");
                sb.Append(table.PrimaryKeyIsClustered ? "CLUSTERED " : "NONCLUSTERED ");
                sb.Append('(');
                foreach (var col in table.Columns.PrimaryKey()) {
                    sb.Append(col.Name.SqlQuote());
                    sb.Append(", ");
                }
                sb.Length -= 2;
                sb.Append(')');
                sb.AppendLine();
            }

            sb.Append(')');
            sb.AppendLine();

            var createTableSql = sb.ToString();
            Database.ExecuteNonQuery(createTableSql);

            foreach (var idx in table.Indices)
                Database.AddIndex(table.Name, idx);
        }

        static public void AddColumns<T>(this ITransformationProvider Database, Action populate = null) => Database.AddColumns(typeof(T), populate);
        static public void AddColumns(this ITransformationProvider Database, Type tableType, Action populate = null)
        {
            var table = tableType.ParseTable().Table;
            var pocoColumns = table.Columns.DefinedInPoco();
            var nonNullables = new List<EColumn>();

            foreach (var col in pocoColumns) {
				if (populate != null && !col.IsNullable && col.DefaultValue == null) {
				    col.IsNullable = true;
					nonNullables.Add(col);
				}
                AddColumn(Database, table, col);
            }

            if (populate != null) {
                populate();
                foreach (var col in nonNullables) {
                    col.IsNullable = false;
                    AlterColumn(Database, table, col);
                }
            }

            foreach (var idx in table.Indices)
                Database.AddIndex(table.Name, idx);
        }


        static public void ChangeColumns<T>(this ITransformationProvider Database, Action populate = null) => Database.ChangeColumns(typeof(T), populate);
        static public void ChangeColumns(this ITransformationProvider Database, Type tableType, Action populate = null)
        {
            var table = tableType.ParseTable().Table;
            var pocoColumns = table.Columns.DefinedInPoco();
            var nonNullables = new List<EColumn>();
            var defaults = Database.GetDefaults(table);

            foreach (var col in pocoColumns) {
                if (defaults.ContainsKey(col.Name))
                    Database.RemoveConstraint(table.Name, defaults[col.Name].Item1);

                if (populate != null && !col.IsNullable && col.DefaultValue == null) {
				    col.IsNullable = true;
					nonNullables.Add(col);
				}
                AlterColumn(Database, table, col);
            }

            if (populate != null) {
                populate();
                foreach (var col in nonNullables) {
                    col.IsNullable = false;
                    AlterColumn(Database, table, col);
                }
            }

            foreach (var col in pocoColumns)
                if (!string.IsNullOrEmpty(col.DefaultValue))
                    Database.ExecuteNonQuery($"ALTER TABLE {table.Name.SqlQuote()} ADD CONSTRAINT DF_{table.Name}_{col.Name} DEFAULT({col.DefaultValue}) FOR {col.Name.SqlQuote()}");
        }


            static private void AddColumn(ITransformationProvider Database, Parsing.Model.Table table, EColumn col)
            => Database.ExecuteNonQuery(BuildAddColumn(table, col));

        static private string BuildAddColumn(Parsing.Model.Table table, EColumn col)
        {
            var sb = new StringBuilder();
            sb.Append("ALTER TABLE ");
            sb.Append(table.Name.SqlQuote());
            sb.Append(" ADD ");
            BuildFullColumnSpec(sb, table, col);
            return sb.ToString();
        }

        static private void AlterColumn(ITransformationProvider Database, Parsing.Model.Table table, EColumn col)
            => Database.ExecuteNonQuery(BuildAlterColumn(table, col));

        static private string BuildAlterColumn(Parsing.Model.Table table, EColumn col)
        {
            var sb = new StringBuilder();
            sb.Append("ALTER TABLE ");
            sb.Append(table.Name.SqlQuote());
            sb.Append(" ALTER COLUMN ");
            BuildBasicColumnSpec(sb, table, col);
            return sb.ToString();
        }

        static private void BuildFullColumnSpec(StringBuilder sb, Parsing.Model.Table table, EColumn col)
        {
            BuildBasicColumnSpec(sb, table, col);

            if (col.DefaultValue != null) {
                sb.Append(" CONSTRAINT ");
                sb.Append("DF_");
                sb.Append(table.Name);
                sb.Append('_');
                sb.Append(col.Name);
                sb.Append(" DEFAULT(");
                sb.Append(col.DefaultValue);
                sb.Append(')');
            }

            if (col.ForeignKey != null) {
                sb.Append(" CONSTRAINT ");
                sb.Append(col.ForeignKey.Name.SqlQuote());
                sb.Append(" FOREIGN KEY REFERENCES ");
                sb.Append(col.ForeignKey.Table.SqlQuote());
                sb.Append('(');
                sb.Append(col.ForeignKey.Column.SqlQuote());
                sb.Append(')');
                if (col.ForeignKey.OnDelete.HasValue) {
                    sb.Append(" ON DELETE ");
                    sb.Append(FkRuleString(col.ForeignKey.OnDelete.Value));
                }
                if (col.ForeignKey.OnUpdate.HasValue) {
                    sb.Append(" ON UPDATE ");
                    sb.Append(FkRuleString(col.ForeignKey.OnUpdate.Value));
                }
            }
        }

        static private string FkRuleString(Rule rule)
        {
            switch (rule) {
                case Rule.None: return "NO ACTION";
                case Rule.Cascade: return "CASCADE";
                case Rule.SetDefault: return "SET DEFAULT";
                case Rule.SetNull: return "SET NULL";
                default: throw new ArgumentException($"Unknown foreign key rule {rule}", nameof(rule));
            }
        }

        static private void BuildBasicColumnSpec(StringBuilder sb, Parsing.Model.Table table, EColumn col)
        {
            sb.Append(col.Name.SqlQuote());
            sb.Append(' ');
            sb.Append(GetSqlTypeString(col));

            if (col.Length.HasValue) {
                sb.Append('(');
                if (col.Length == int.MaxValue)
                    sb.Append("MAX");
                else
                    sb.Append(col.Length);
                sb.Append(')');
            }
            else if (col.Precision?.Precision > 0 && col.Precision?.Scale > 0) {
                sb.Append('(');
                sb.Append(col.Precision.Precision);
                sb.Append(',');
                sb.Append(col.Precision.Scale);
                sb.Append(')');
            }
            else if (col.Precision?.Precision > 0 || col.Precision?.Scale > 0) {
                sb.Append('(');
                sb.Append(Math.Max(col.Precision?.Precision ?? 0, col.Precision?.Scale ?? 0));
                sb.Append(')');
            }

            if (col.AutoIncrement != null) {
                sb.Append(" IDENTITY(");
                sb.Append(col.AutoIncrement.Seed);
                sb.Append(',');
                sb.Append(col.AutoIncrement.Step);
                sb.Append(')');
            }

            if (col.IsSparse)
                sb.Append(" SPARSE");

            sb.Append(col.IsNullable ? " NULL" : " NOT NULL");
        }


        static private string GetSqlTypeString(EColumn col)
        {
            if (col.CustomType != null)
                return col.CustomType;

            switch (col.Type) {
                case DbType.AnsiString: return "VARCHAR";
                case DbType.AnsiStringFixedLength: return "CHAR";
                case DbType.Binary: return col.IsFixedLength ? "BINARY" : "VARBINARY";
                case DbType.Byte: return "TINYINT";
                case DbType.Currency: return "SMALLMONEY";
                case DbType.Boolean: return "BIT";
                case DbType.Date: return "DATE";
                case DbType.DateTime: return "DATETIME";
                case DbType.DateTime2: return "DATETIME2";
                case DbType.DateTimeOffset: return "DATETIMEOFFSET";
                case DbType.Decimal: return "DECIMAL";
                case DbType.Double: return "FLOAT";
                case DbType.Guid: return "UNIQUEIDENTIFIER";
                case DbType.Int16: return "SMALLINT";
                case DbType.Int32: return "INT";
                case DbType.Int64: return "BIGINT";
                case DbType.Single: return "FLOAT";
                case DbType.String: return "NVARCHAR";
                case DbType.StringFixedLength: return "NCHAR";
                case DbType.Time: return "TIME";
                case DbType.Xml: return "XML";
                default: throw new ArgumentException($"{col.Type} is not a supported custom type.", nameof(col.Type));
            }
        }

        static private IDictionary<string, Tuple<string, string>> GetDefaults(this ITransformationProvider Database, Parsing.Model.Table table)
        {
            var dict = new Dictionary<string, Tuple<string, string>>(StringComparer.OrdinalIgnoreCase);
            var sql = string.Format(GetDefaultsSql, table.Name);
            using (var reader = Database.ExecuteQuery(sql))
                while (reader.Read())
                    dict.Add(reader.GetString(0), new Tuple<string, string>(reader.GetString(1), reader.GetString(2)));

            return dict;
        }

        private const string GetDefaultsSql =
@"SELECT c.name, dc.name, dc.definition
FROM sys.all_columns c
INNER JOIN sys.tables t ON t.object_id=c.object_id
INNER JOIN sys.default_constraints dc ON c.default_object_id=dc.object_id
WHERE t.name='{0}'";

    }
}
