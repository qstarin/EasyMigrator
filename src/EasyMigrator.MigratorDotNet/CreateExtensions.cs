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
        static public void AddTable(this ITransformationProvider Database, Type tableType) => Database.AddTable(tableType, Parsing.Parser.Default);
        static public void AddTable<T>(this ITransformationProvider Database, Parsing.Parser parser) => Database.AddTable(typeof(T), parser);
        static public void AddTable(this ITransformationProvider Database, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTableType(tableType).Table;
            var pocoColumns = table.Columns;
            var columns = new List<Column>();
            //var primaryKeyColumns = new List<Column>();
            foreach (var col in pocoColumns) {
                if (col.AutoIncrement != null && (col.AutoIncrement.Seed > 1 || col.AutoIncrement.Step > 1))
                    throw new NotImplementedException("AutoIncrement Seeds or Steps other than 1 are not supported for MigratorDotNet.");

                var isPk = col.IsPrimaryKey;
                if (isPk)
                    col.IsPrimaryKey = false;

                var c = BuildColumn(col);
                columns.Add(c);

                if (isPk)
                    col.IsPrimaryKey = true;
            }

            Database.AddTable(table.Name, columns.ToArray());
            Database.AddPrimaryKey(table.PrimaryKeyName, table.Name, table.PrimaryKeyColumns.Select(c => c.Name).ToArray());

            foreach (var col in pocoColumns.Where(c => c.ForeignKey != null)) {
                var fk = col.ForeignKey;
                Database.AddForeignKey(fk.Name, table.Name, col.Name, fk.Table, fk.Column);
            }
            
            foreach (var col in pocoColumns.Where(c => c.Index != null)) {
                var idx = col.Index;
                Database.CreateIndex(idx.Unique, idx.Clustered, idx.Name, table.Name, col.Name);
            }
        }

        static public void AddColumns<T>(this ITransformationProvider Database, Action populate = null) => Database.AddColumns(typeof(T), populate);
        static public void AddColumns(this ITransformationProvider Database, Type tableType, Action populate = null) => Database.AddColumns(tableType, Parsing.Parser.Default, populate);
        static public void AddColumns<T>(this ITransformationProvider Database, Parsing.Parser parser, Action populate = null) => Database.AddColumns(typeof(T), parser, populate);
        static public void AddColumns(this ITransformationProvider Database, Type tableType, Parsing.Parser parser, Action populate = null)
        {
            var table = parser.ParseTableType(tableType).Table;
            var pocoColumns = table.Columns.DefinedInPoco();
            var nonNullables = new List<EColumn>();
            foreach (var col in pocoColumns) {
                if (col.AutoIncrement != null && (col.AutoIncrement.Seed > 1 || col.AutoIncrement.Step > 1))
                    throw new NotImplementedException("AutoIncrement Seeds or Steps other than 1 are not supported for MigratorDotNet.");

				if (populate != null && !col.IsNullable && col.DefaultValue == null) {
				    col.IsNullable = true;
					nonNullables.Add(col);
				}

                Database.AddColumn(table.Name, BuildColumn(col));
            }
            
            if (populate != null) {
                populate();
                foreach (var col in nonNullables) {
                    col.IsNullable = false;
                    Database.ChangeColumn(table.Name, BuildColumn(col));
                }
            }

            foreach (var col in pocoColumns.Where(c => c.ForeignKey != null)) {
                var fk = col.ForeignKey;
                Database.AddForeignKey(fk.Name, table.Name, col.Name, fk.Table, fk.Column);
            }
            
            foreach (var col in pocoColumns.Where(c => c.Index != null)) {
                var idx = col.Index;
                Database.CreateIndex(idx.Unique, idx.Clustered, idx.Name, table.Name, col.Name);
            }
        }

        static private Column BuildColumn(EColumn col)
        {
            var c = new Column(col.Name, col.Type);
            ColumnProperty cp = ColumnProperty.None;

            if (col.IsPrimaryKey)
                cp |= ColumnProperty.PrimaryKey;

            if (col.AutoIncrement != null)
                cp |= ColumnProperty.Identity;

            cp |= col.IsNullable ? ColumnProperty.Null : ColumnProperty.NotNull;
            c.ColumnProperty = cp;

            if (col.DefaultValue != null)
                c.DefaultValue = col.DefaultValue;

            if (col.Length.HasValue)
                c.Size = col.Length.Value == int.MaxValue ? 0x3fffffff : col.Length.Value;
            // http://code.google.com/p/migratordotnet/issues/detail?id=130
            // using this sets the column type to ntext instead of nvarchar

            if (col.Type == DbType.Decimal && col.Precision != null) {
                // MigratorDotNet doesn't seem to support precision
                c.Size = col.Precision.Scale;
            }


            return c;
        }

        static private void CreateIndex(this ITransformationProvider Database, bool unique, bool clustered, string indexName, string table, params string[] columns)
            => Database.ExecuteNonQuery($"CREATE {(unique ? "UNIQUE " : "")}{(clustered ? "CLUSTERED" : "NONCLUSTERED")} INDEX {indexName} ON [{table}] ({string.Join(", ", columns)})");
    }
}
