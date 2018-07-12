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

                if (col.IsSparse)
                    unsupportedTypeColumns.Add(col);

                if (UnsupportedTypes.ContainsKey(col.Type)) {
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
                        table.Columns.WithCustomAutoIncrement().Select(c => BuildColumnWithCustomIdentitySql(c.Name, c.Type, c.AutoIncrement.Seed, c.AutoIncrement.Step, c.IsNullable))) + 
                    ")");

                foreach (var col in columns)
                    Database.AddColumn(table.Name.SqlQuote(), col);
            }
            else
                Database.AddTable(table.Name.SqlQuote(), columns.ToArray());

            Database.AddPrimaryKey(table.Name, table.PrimaryKeyName, table.PrimaryKeyIsClustered, table.Columns.PrimaryKey().Select(c => c.Name).ToArray());

            foreach (var col in table.Columns.MaxLength())
                AlterToMaxLength(Database, table.Name, col.Name, col.Type, col.IsNullable);

            foreach (var col in table.Columns.WithPrecision().Except(unsupportedTypeColumns))
                AlterForUnsupportedType(Database, table.Name, col.Name, col.Type, col.Precision.Precision, col.Precision.Scale, col.IsNullable, col.IsSparse);

            foreach (var col in unsupportedTypeColumns)
                AlterForUnsupportedType(Database, table.Name, col.Name, col.Type, col.Precision?.Precision, col.Precision?.Scale, col.IsNullable, col.IsSparse);

            foreach (var col in table.Columns.ForeignKeys()) {
                var fk = col.ForeignKey;
                Database.AddForeignKey(fk.Name.SqlQuote(), table.Name.SqlQuote(), col.Name.SqlQuote(), fk.Table.SqlQuote(), fk.Column.SqlQuote());
            }

            foreach (var idx in table.Indices)
                Database.AddIndex(table.Name, idx.Name, idx.Unique, idx.Clustered, idx.Columns.Select(c => c.ColumnNameWithDirection).ToArray(), idx.Includes?.Select(c => c.ColumnName).ToArray());
        }

        static public void AddColumns<T>(this ITransformationProvider Database, Action populate = null) => Database.AddColumns(typeof(T), populate);
        static public void AddColumns(this ITransformationProvider Database, Type tableType, Action populate = null)
        {
            var table = tableType.ParseTable().Table;
            var pocoColumns = table.Columns.DefinedInPoco();
            var nonNullables = new List<EColumn>();
            var unsupportedTypeColumns = new List<EColumn>();

            foreach (var col in pocoColumns) {
				if (populate != null && !col.IsNullable && col.DefaultValue == null) {
				    col.IsNullable = true;
					nonNullables.Add(col);
				}

                var c = BuildColumn(col);

                if (col.IsSparse)
                    unsupportedTypeColumns.Add(col);

                if (UnsupportedTypes.ContainsKey(col.Type)) {
                    unsupportedTypeColumns.Add(col);
                    c.Type = UnsupportedTypes[col.Type];
                }

                if (col.IsCustomAutoIncrement())
                    AddColumnWithCustomIdentity(Database, table.Name, col.Name, col.Type, col.AutoIncrement.Seed, col.AutoIncrement.Step, col.IsNullable);
                else
                    Database.AddColumn(table.Name.SqlQuote(), c);
            }

            foreach (var col in pocoColumns.MaxLength())
                AlterToMaxLength(Database, table.Name, col.Name, col.Type, col.IsNullable);

            foreach (var col in pocoColumns.WithPrecision())
                AlterForUnsupportedType(Database, table.Name, col.Name, col.Type, col.Precision.Precision, col.Precision.Scale, col.IsNullable, col.IsSparse);

            foreach (var col in unsupportedTypeColumns)
                AlterForUnsupportedType(Database, table.Name, col.Name, col.Type, col.Precision?.Precision, col.Precision?.Scale, col.IsNullable, col.IsSparse);

            if (populate != null) {
                populate();
                foreach (var col in nonNullables) {
                    col.IsNullable = false;
                    if (table.Columns.MaxLength().Contains(col))
                        AlterToMaxLength(Database, table.Name, col.Name, col.Type, col.IsNullable);
                    else
                        Database.ChangeColumn(table.Name.SqlQuote(), BuildColumn(col));
                }
            }

            foreach (var col in pocoColumns.ForeignKeys()) {
                var fk = col.ForeignKey;
                Database.AddForeignKey(fk.Name.SqlQuote(), table.Name.SqlQuote(), col.Name.SqlQuote(), fk.Table.SqlQuote(), fk.Column.SqlQuote());
            }

            foreach (var idx in table.Indices)
                Database.AddIndex(table.Name, idx.Name, idx.Unique, idx.Clustered, idx.Columns.Select(c => c.ColumnNameWithDirection).ToArray(), idx.Includes?.Select(c => c.ColumnName).ToArray());
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
        static private void AlterToMaxLength(ITransformationProvider Database, string tableName, string columnName, DbType dbType, bool isNullable)
            => Database.ExecuteNonQuery($"ALTER TABLE {tableName.SqlQuote()} ALTER COLUMN {columnName.SqlQuote()} {(dbType == DbType.AnsiString ? "" : "N")}VARCHAR(MAX) {(isNullable ? "" : "NOT ")}NULL");

        // Migrator.Net doesn't support some types or setting the precision so we do it manually with SQL
        static private void AlterForUnsupportedType(ITransformationProvider Database, string tableName, string columnName, DbType dbType, int? precision, int? scale, bool isNullable, bool isSparse)
            => Database.ExecuteNonQuery($"ALTER TABLE {tableName.SqlQuote()} ALTER COLUMN {columnName.SqlQuote()} " +
                GetSqlTypeString(dbType) + 
                (precision.HasValue && precision > 0 && scale.HasValue && scale > 0
                    ? $"({precision}, {scale})"
                    : (precision.HasValue && precision > 0) || (scale.HasValue && scale > 0)
                        ? $"({Math.Max(precision ?? 0, scale ?? 0)})"
                        : "")
                + (isSparse  ? " SPARSE" : "")
                + $" {(isNullable ? "" : "NOT ")}NULL");

        // Migrator.Net also doesn't support identity seed and step values other than the defaults of 1
        static private void AddColumnWithCustomIdentity(ITransformationProvider Database, string tableName, string columnName, DbType dbType, long seed, long step, bool isNullable)
            => Database.ExecuteNonQuery($"ALTER TABLE {tableName.SqlQuote()} ADD COLUMN {BuildColumnWithCustomIdentitySql(columnName, dbType, seed, step, isNullable)}");

        static private string BuildColumnWithCustomIdentitySql(string columnName, DbType dbType, long seed, long step, bool isNullable)
            => $"{columnName.SqlQuote()} {GetSqlTypeString(dbType)} IDENTITY({seed}, {step}) {(isNullable ? "" : "NOT ")}NULL";

        static private string GetSqlTypeString(DbType dbType)
        {
            switch (dbType) {
                case DbType.Byte: return "TINYINT";
                case DbType.Int16: return "SMALLINT";
                case DbType.Int32: return "INT";
                case DbType.Int64: return "BIGINT";
                case DbType.DateTime2: return "DATETIME2";
                case DbType.DateTimeOffset: return "DATETIMEOFFSET";
                case DbType.Decimal: return "DECIMAL";
                default: throw new ArgumentException($"{dbType} is not a supported custom type.", nameof(dbType));
            }
        }
    }
}
