using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EasyMigrator.Extensions;
using Migrator.Framework;


namespace EasyMigrator
{
    static public class CreateIndexExtensions
    {
        static public void AddIndex<TTable>(this ITransformationProvider Database, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(new Index<TTable>(columns));

        static public void AddIndex<TTable>(this ITransformationProvider Database, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null)
            => Database.AddIndex(new Index<TTable>(columns, includes));

        static public void AddUniqueIndex<TTable>(this ITransformationProvider Database, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(new Index<TTable>(columns) { Unique = true });

        static public void AddUniqueIndex<TTable>(this ITransformationProvider Database, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null)
            => Database.AddIndex(new Index<TTable>(columns, includes) { Unique = true });


        static public void AddIndex<TTable>(this ITransformationProvider Database, params string[] columnNamesWithDirection)
            => Database.AddIndex(typeof(TTable).ParseTable().Table.Name, (Parsing.Model.IIndex)new Index(columnNamesWithDirection));

        static public void AddIndex<TTable>(this ITransformationProvider Database, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex(typeof(TTable).ParseTable().Table.Name, (Parsing.Model.IIndex)new Index(columnNamesWithDirection, includes));

        static public void AddUniqueIndex<TTable>(this ITransformationProvider Database, params string[] columnNamesWithDirection)
            => Database.AddIndex(typeof(TTable).ParseTable().Table.Name, (Parsing.Model.IIndex)new Index(columnNamesWithDirection) { Unique = true });

        static public void AddUniqueIndex<TTable>(this ITransformationProvider Database, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex(typeof(TTable).ParseTable().Table.Name, (Parsing.Model.IIndex)new Index(columnNamesWithDirection, includes) { Unique = true });


        static public void AddIndex(this ITransformationProvider Database, string table, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, (Parsing.Model.IIndex)new Index(columnNamesWithDirection));

        static public void AddIndex(this ITransformationProvider Database, string table, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex(table, (Parsing.Model.IIndex)new Index(columnNamesWithDirection, includes));

        static public void AddUniqueIndex(this ITransformationProvider Database, string table, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, (Parsing.Model.IIndex)new Index(columnNamesWithDirection) { Unique = true });

        static public void AddUniqueIndex(this ITransformationProvider Database, string table, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex(table, (Parsing.Model.IIndex)new Index(columnNamesWithDirection, includes) { Unique = true });


        static public void AddIndex<TTable>(this ITransformationProvider Database, Index<TTable> index)
        {
            var context = typeof(TTable).ParseTable();
            var cols = index.Columns
                            .Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                            .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
            var incl = index.Includes?
                            .Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                            .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));

            Database.AddIndex(context.Table.Name,
                             (Parsing.Model.IIndex)new Index(cols.ToArray(), incl?.ToArray()) {
                                 Name = index.Name,
                                 Unique = index.Unique,
                                 Clustered = index.Clustered,
                                 Where = index.Where,
                                 With = index.With,
                             });
        }

        static public void AddIndex(this ITransformationProvider Database, string table, Index index) => Database.AddIndex(table, (Parsing.Model.IIndex)index);
        static internal void AddIndex(this ITransformationProvider Database, string table, Parsing.Model.IIndex index)
            => Database.ExecuteNonQuery(
                $"CREATE {(index.Unique ? "UNIQUE " : "")}{(index.Clustered ? "CLUSTERED" : "NONCLUSTERED")} " +
                $"INDEX {(index.Name ?? Parsing.Parser.Current.Conventions.IndexNameByTableAndColumnNames(table, index.Columns.Select(c => c.ColumnName))).SqlQuote()} " + 
                $"ON {table.SqlQuote()} ({string.Join(", ", QuoteColumns(index.Columns.Select(c => c.ColumnNameWithDirection)))})" +
                (index.Includes == null || index.Includes.Length == 0 ? "" : $" INCLUDE ({string.Join(", ", index.Includes.Select(c => c.ColumnName.SqlQuote()))})") +
                (string.IsNullOrEmpty(index.Where) ? "" : $" WHERE {index.Where}") +
                (string.IsNullOrEmpty(index.With) ? "" : $" WITH ({index.With})"));


        static private IEnumerable<string> RemoveDirection(IEnumerable<string> columnNamesWithDirection)
        {
            foreach (var c in columnNamesWithDirection) {
                if (c.EndsWith(" ASC"))
                    yield return c.Substring(0, c.Length - " ASC".Length);
                else if (c.EndsWith(" DESC"))
                    yield return c.Substring(0, c.Length - " DESC".Length);
                else
                    yield return c;
            }
        }

        static private IEnumerable<string> QuoteColumns(IEnumerable<string> columnNamesWithDirection)
        {
            foreach (var c in columnNamesWithDirection) {
                if (c.EndsWith(" ASC"))
                    yield return c.Substring(0, c.Length - " ASC".Length).SqlQuote() + " ASC";
                else if (c.EndsWith(" DESC"))
                    yield return c.Substring(0, c.Length - " DESC".Length).SqlQuote() + " DESC";
                else
                    yield return c.SqlQuote();
            }
        }
    }
}
