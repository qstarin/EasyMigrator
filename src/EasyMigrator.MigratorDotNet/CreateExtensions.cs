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
        static private IDictionary<DbType, DbType> UnsupportedTypes = new Dictionary<DbType, DbType> {
            { DbType.DateTime2, DbType.DateTime },
            { DbType.DateTimeOffset, DbType.DateTime },
            { DbType.Binary, DbType.Binary },
        };

        static public void AddTable<T>(this ITransformationProvider Database) => Database.AddTable(typeof(T));
        static public void AddTable(this ITransformationProvider Database, Type tableType)
        {
            var table = tableType.ParseTable().Table;
            var columns = new List<Column>();
            var unsupportedTypeColumns = new List<EColumn>();
            foreach (var col in table.Columns.WithoutCustomAutoIncrement()) {
                // do this temporarily because we want to create the PK ourselves later in order to set the name and support unclustered and composite PK's
                var isPk = col.IsPrimaryKey;
                if (isPk)
                    col.IsPrimaryKey = false;

                var c = BuildColumn(col);

                if (col.IsSparse || col.CustomType != null)
                    unsupportedTypeColumns.Add(col);
                else if (UnsupportedTypes.ContainsKey(col.Type)) {
                    unsupportedTypeColumns.Add(col);
                    c.Type = UnsupportedTypes[col.Type];
                }

                columns.Add(c);

                if (isPk)
                    col.IsPrimaryKey = true;
            }

            if (table.Columns.WithCustomAutoIncrement().Any()) {
                Database.ExecuteNonQuery(
                    $"CREATE TABLE {table.Name.SqlQuote()} (" + 
                    string.Join(", ", 
                        table.Columns.WithCustomAutoIncrement().Select(BuildColumnSpec)) + 
                    ")");

                foreach (var col in columns)
                    Database.AddColumn(table.Name.SqlQuote(), col);
            }
            else
                Database.AddTable(table.Name.SqlQuote(), columns.ToArray());

            Database.AddPrimaryKey(table.Name, table.PrimaryKeyName, table.PrimaryKeyIsClustered, table.Columns.PrimaryKey().Select(c => c.Name).ToArray());

            foreach (var col in table.Columns.MaxLength())
                AlterColumn(Database, table.Name, col);

            foreach (var col in table.Columns.WithPrecision().Except(unsupportedTypeColumns))
                AlterColumn(Database, table.Name, col);

            foreach (var col in unsupportedTypeColumns)
                AlterColumn(Database, table.Name, col);

            foreach (var col in table.Columns.ForeignKeys()) {
                var fk = col.ForeignKey;
                Database.AddForeignKey(fk.Name.SqlQuote(), table.Name.SqlQuote(), col.Name.SqlQuote(), fk.Table.SqlQuote(), fk.Column.SqlQuote());
            }

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
                AddColumn(Database, table.Name, col);
            }

            if (populate != null) {
                populate();
                foreach (var col in nonNullables) {
                    col.IsNullable = false;
                    AlterColumn(Database, table.Name, col);
                }
            }

            foreach (var col in pocoColumns.ForeignKeys()) {
                var fk = col.ForeignKey;
                Database.AddForeignKey(fk.Name.SqlQuote(), table.Name.SqlQuote(), col.Name.SqlQuote(), fk.Table.SqlQuote(), fk.Column.SqlQuote());
            }

            foreach (var idx in table.Indices)
                Database.AddIndex(table.Name, idx);
        }

        static private Column BuildColumn(EColumn col)
        {
            var c = new Column(col.Name.SqlQuote(), col.Type);
            ColumnProperty cp = ColumnProperty.None;

            // don't set this property on the columns so we can create the primary key ourselves later to set the same and support nonclustered and composite PK's
            //if (col.IsPrimaryKey)
            //    cp |= ColumnProperty.PrimaryKey;

            if (col.IsDefaultAutoIncrement())
                cp |= ColumnProperty.Identity;

            if (new [] { DbType.UInt16, DbType.UInt32, DbType.UInt64 }.Contains(col.Type))
                cp |= ColumnProperty.Unsigned;

            cp |= col.IsNullable ? ColumnProperty.Null : ColumnProperty.NotNull;
            c.ColumnProperty = cp;

            if (col.DefaultValue != null)
                c.DefaultValue = col.DefaultValue;

            if (col.Length.HasValue)
                c.Size = col.Length.Value;

            // Migrator.Net doesn't support setting the precision so we do it manually with SQL
            //if (col.Type == DbType.Decimal && col.Precision != null)
            //    c.Size = col.Precision.Scale;

            return c;
        }


        // Migrator.Net can't create a [n]varchar(max) column
        // http://code.google.com/p/migratordotnet/issues/detail?id=130
        // using this sets the column type to ntext instead of nvarchar
        // so, we work around it

        static private void AddColumn(ITransformationProvider Database, string table, EColumn col)
            => Database.ExecuteNonQuery(BuildAddColumn(table, col));

        static private void AlterColumn(ITransformationProvider Database, string table, EColumn col)
            => Database.ExecuteNonQuery(BuildAlterColumn(table, col));

        static private string BuildAddColumn(string table, EColumn col)
            => $"ALTER TABLE {table.SqlQuote()} ADD {BuildColumnSpec(col)}";

        static private string BuildAlterColumn(string table, EColumn col)
            => $"ALTER TABLE {table.SqlQuote()} ALTER COLUMN {BuildColumnSpec(col)}";

        static private string BuildColumnSpec(EColumn col)
            => $"{col.Name.SqlQuote()} " + GetSqlTypeString(col) +
               (col.Length.HasValue ? $"({(col.Length == int.MaxValue ? "MAX" : col.Length.ToString())})" : "") +
               (col.Precision?.Precision > 0 && col.Precision?.Scale > 0 ? $"({col.Precision.Precision}, {col.Precision.Scale})"
               : (col.Precision?.Precision > 0) || (col.Precision?.Scale > 0) ? $"({Math.Max(col.Precision?.Precision ?? 0, col.Precision?.Scale ?? 0)})"
               : "")
               + (col.AutoIncrement == null ? "" : $" IDENTITY({col.AutoIncrement.Seed}, {col.AutoIncrement.Step})")
               + (col.IsSparse ? " SPARSE" : "")
               + (col.IsNullable ? " NULL" : " NOT NULL");

        static private string GetSqlTypeString(EColumn col)
        {
            if (col.CustomType != null)
                return col.CustomType;

            switch (col.Type) {
                case DbType.AnsiString: return "VARCHAR";
                case DbType.AnsiStringFixedLength: return "CHAR";
                case DbType.Binary: return col.IsFixedLength ? "BINARY" : "VARBINARY";
                case DbType.Byte: return "TINYINT";
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
    }
}
