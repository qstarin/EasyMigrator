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
        static public void AddTable<T>(this ITransformationProvider db) => db.AddTable(typeof(T));
        static public void AddTable(this ITransformationProvider db, Type tableType) => db.AddTable(tableType, Parsing.Parser.Default);
        static public void AddTable<T>(this ITransformationProvider db, Parsing.Parser parser) => db.AddTable(typeof(T), parser);
        static public void AddTable(this ITransformationProvider db, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTableType(tableType);
            var pocoColumns = table.Columns;
            var columns = new List<Column>();
            foreach (var col in pocoColumns) {
                if (col.AutoIncrement != null && (col.AutoIncrement.Seed > 1 || col.AutoIncrement.Step > 1))
                    throw new NotImplementedException("AutoIncrement Seeds or Steps other than 1 are not supported for MigratorDotNet.");

                columns.Add(BuildColumn(col));
            }

            db.AddTable(table.Name, columns.ToArray());
            //db.AddPrimaryKey(table.PrimaryKeyName, table.Name, table.PrimaryKeyColumns.Select(c => c.Name).ToArray());

            foreach (var col in pocoColumns.Where(c => c.ForeignKey != null)) {
                var fk = col.ForeignKey;
                db.AddForeignKey(fk.Name, table.Name, col.Name, fk.Table, fk.Column);
            }
            
            foreach (var col in pocoColumns.Where(c => c.Index != null)) {
                var idx = col.Index;
                db.CreateIndex(idx.Unique, idx.Clustered, idx.Name, table.Name, col.Name);
            }
        }

        static public void AddColumns<T>(this ITransformationProvider db) => db.AddColumns(typeof(T));
        static public void AddColumns(this ITransformationProvider db, Type tableType) => db.AddColumns(tableType, Parsing.Parser.Default);
        static public void AddColumns<T>(this ITransformationProvider db, Parsing.Parser parser) => db.AddColumns(typeof(T), parser);
        static public void AddColumns(this ITransformationProvider db, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTableType(tableType);
            var pocoColumns = table.Columns.DefinedInPoco();
            var columns = new List<Column>();
            foreach (var col in pocoColumns) {
                if (col.AutoIncrement != null && (col.AutoIncrement.Seed > 1 || col.AutoIncrement.Step > 1))
                    throw new NotImplementedException("AutoIncrement Seeds or Steps other than 1 are not supported for MigratorDotNet.");

                columns.Add(BuildColumn(col));
            }

            foreach (var col in columns)
                db.AddColumn(table.Name, col);

            foreach (var col in pocoColumns.Where(c => c.ForeignKey != null)) {
                var fk = col.ForeignKey;
                db.AddForeignKey(fk.Name, table.Name, col.Name, fk.Table, fk.Column);
            }
            
            foreach (var col in pocoColumns.Where(c => c.Index != null)) {
                var idx = col.Index;
                db.CreateIndex(idx.Unique, idx.Clustered, idx.Name, table.Name, col.Name);
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
                c.Size = col.Length.Value == int.MaxValue ? /*-1*/ 0x3fffffff : col.Length.Value; // http://code.google.com/p/migratordotnet/issues/detail?id=130

            // TODO: decimal scale isn't handled, and not sure if precision is right
            if (col.Type == DbType.Decimal && col.Precision != null) {
                c.Size = col.Precision.Scale;
            }


            return c;
        }

        static private void CreateIndex(this ITransformationProvider db, bool unique, bool clustered, string indexName, string table, params string[] columns)
            => db.ExecuteNonQuery($"CREATE {(unique ? "UNIQUE " : "")}{(clustered ? "CLUSTERED" : "NONCLUSTERED")} INDEX {indexName} ON [{table}] ({string.Join(", ", columns)})");
    }
}
