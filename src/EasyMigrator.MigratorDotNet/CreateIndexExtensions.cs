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
            => Database.AddIndex(table,
                                (Parsing.Model.IIndex)new Index(columnNamesWithDirection, includes) {
                                    Name = indexName,
                                    Unique = unique,
                                    Clustered = clustered,
                                });

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
