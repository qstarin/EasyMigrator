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
            => Database.AddIndex(null, false, false, columns, null, null);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, false, false, columns, null, null);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(null, unique, clustered, columns, null, null);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, unique, clustered, columns, null, null);

        static public void AddIndex<TTable>(this ITransformationProvider Database, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(null, false, false, columns, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(indexName, false, false, columns, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(null, unique, clustered, columns, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(indexName, unique, clustered, columns.Select(c => new IndexColumn<TTable>(c)).ToArray(), includes?.Select(c => new IndexColumn<TTable>(c)).ToArray(), parser);


        static public void AddIndex<TTable>(this ITransformationProvider Database, IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(null, false, false, columns, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(indexName, false, false, columns, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(null, unique, clustered, columns, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes = null, Parsing.Parser parser = null)
        {
            var context = (parser ?? Parsing.Parser.Default).ParseTableType(typeof(TTable));
            var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                              .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
            var incls = includes?.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                                 .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
            Database.AddIndex<TTable>(indexName, unique, clustered, cols.ToArray(), incls?.ToArray(), parser);
        }


        static public void AddIndex<TTable>(this ITransformationProvider Database, IndexColumn[] columns, IndexColumn[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex<TTable>(null, false, false, columns, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, IndexColumn[] columns, IndexColumn[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex<TTable>(indexName, false, false, columns, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, IndexColumn[] columns, IndexColumn[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex<TTable>(null, unique, clustered, columns, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, IndexColumn[] columns, IndexColumn[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex<TTable>(indexName, unique, clustered, columns.Select(c => c.ColumnNameWithDirection).ToArray(), includes?.Select(c => c.ColumnNameWithDirection).ToArray(), parser);


        static public void AddIndex<TTable>(this ITransformationProvider Database, string[] columnNamesWithDirection, string[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex<TTable>(null, false, false, columnNamesWithDirection, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, string[] columnNamesWithDirection, string[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex<TTable>(indexName, false, false, columnNamesWithDirection, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, string[] columnNamesWithDirection, string[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex<TTable>(null, unique, clustered, columnNamesWithDirection, includes, parser);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, string[] columnNamesWithDirection, string[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex((parser ?? Parsing.Parser.Default).ParseTableType(typeof(TTable)).Table.Name, indexName, unique, clustered, columnNamesWithDirection, includes, parser);


        static public void AddIndex(this ITransformationProvider Database, string table, string[] columnNamesWithDirection, string[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(table, null, false, false, columnNamesWithDirection, includes, parser);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, string[] columnNamesWithDirection, string[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(table, indexName, false, false, columnNamesWithDirection, includes, parser);

        static public void AddIndex(this ITransformationProvider Database, string table, bool unique, bool clustered, string[] columnNamesWithDirection, string[] includes = null, Parsing.Parser parser = null)
            => Database.AddIndex(table, null, unique, clustered, columnNamesWithDirection, includes, parser);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, bool unique, bool clustered, string[] columnNamesWithDirection, string[] includes = null, Parsing.Parser parser = null)
            => Database.ExecuteNonQuery(
                $"CREATE {(unique ? "UNIQUE " : "")}{(clustered ? "CLUSTERED" : "NONCLUSTERED")} " +
                $"INDEX {indexName ?? (parser ?? Parsing.Parser.Default).Conventions.IndexNameByTableAndColumnNames(table, RemoveDirection(columnNamesWithDirection))} " + 
                $"ON {SqlReservedWords.Quote(table)} ({string.Join(", ", QuoteColumns(columnNamesWithDirection))})" +
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
                    yield return SqlReservedWords.Quote(c.Substring(0, c.Length - " ASC".Length)) + " ASC";
                else if (c.EndsWith(" DESC"))
                    yield return SqlReservedWords.Quote(c.Substring(0, c.Length - " DESC".Length)) + " DESC";
                else
                    yield return SqlReservedWords.Quote(c);
            }
        }
    }
}
