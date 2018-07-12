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
            => Database.AddIndex(null, false, false, columns, null);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, false, false, columns, null);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(null, unique, clustered, columns, null);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, unique, clustered, columns, null);

        static public void AddIndex<TTable>(this ITransformationProvider Database, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null)
            => Database.AddIndex(null, false, false, columns, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null)
            => Database.AddIndex(indexName, false, false, columns, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null)
            => Database.AddIndex(null, unique, clustered, columns, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null)
            => Database.AddIndex(indexName, unique, clustered, columns.Select(c => new IndexColumn<TTable>(c)).ToArray(), includes?.Select(c => new IndexColumn<TTable>(c)).ToArray());


        static public void AddIndex<TTable>(this ITransformationProvider Database, IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes = null)
            => Database.AddIndex(null, false, false, columns, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes = null)
            => Database.AddIndex(indexName, false, false, columns, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes = null)
            => Database.AddIndex(null, unique, clustered, columns, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes = null)
        {
            var context = typeof(TTable).ParseTable();
            var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                              .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
            var incls = includes?.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                                 .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
            Database.AddIndex<TTable>(indexName, unique, clustered, cols.ToArray(), incls?.ToArray());
        }


        static public void AddIndex<TTable>(this ITransformationProvider Database, IndexColumn[] columns, IndexColumn[] includes = null)
            => Database.AddIndex<TTable>(null, false, false, columns, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, IndexColumn[] columns, IndexColumn[] includes = null)
            => Database.AddIndex<TTable>(indexName, false, false, columns, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, IndexColumn[] columns, IndexColumn[] includes = null)
            => Database.AddIndex<TTable>(null, unique, clustered, columns, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, IndexColumn[] columns, IndexColumn[] includes = null)
            => Database.AddIndex<TTable>(indexName, unique, clustered, columns.Select(c => c.ColumnNameWithDirection).ToArray(), includes?.Select(c => c.ColumnNameWithDirection).ToArray());


        static public void AddIndex<TTable>(this ITransformationProvider Database, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex<TTable>(null, false, false, columnNamesWithDirection, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex<TTable>(indexName, false, false, columnNamesWithDirection, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex<TTable>(null, unique, clustered, columnNamesWithDirection, includes);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex(typeof(TTable).ParseTable().Table.Name, indexName, unique, clustered, columnNamesWithDirection, includes);


        static public void AddIndex(this ITransformationProvider Database, string table, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex(table, null, false, false, columnNamesWithDirection, includes);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex(table, indexName, false, false, columnNamesWithDirection, includes);

        static public void AddIndex(this ITransformationProvider Database, string table, bool unique, bool clustered, string[] columnNamesWithDirection, string[] includes = null)
            => Database.AddIndex(table, null, unique, clustered, columnNamesWithDirection, includes);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, bool unique, bool clustered, string[] columnNamesWithDirection, string[] includes = null)
            => Database.ExecuteNonQuery(
                $"CREATE {(unique ? "UNIQUE " : "")}{(clustered ? "CLUSTERED" : "NONCLUSTERED")} " +
                $"INDEX {(indexName ?? Parsing.Parser.Current.Conventions.IndexNameByTableAndColumnNames(table, RemoveDirection(columnNamesWithDirection))).SqlQuote()} " + 
                $"ON {table.SqlQuote()} ({string.Join(", ", QuoteColumns(columnNamesWithDirection))})" +
                (includes == null || includes.Length == 0 ? "" : $" INCLUDE ({string.Join(", ", QuoteColumns(RemoveDirection(includes)))})"));

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
