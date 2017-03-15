using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EasyMigrator.Extensions;
using Migrator.Framework;


namespace EasyMigrator.MigratorDotNet
{
    static public class IndexExtensions
    {
        static public void AddIndex<TTable>(this ITransformationProvider Database, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(false, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(false, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(null, unique, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(null, unique, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, false, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, false, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, unique, false, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, unique, false, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(null, unique, clustered, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(null, unique, clustered, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, unique, clustered, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
            => Database.AddIndex(indexName, unique, clustered, parser, columns.Select(c => new IndexColumn<TTable>(c)).ToArray());


        static public void AddIndex<TTable>(this ITransformationProvider Database, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(false, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, Parsing.Parser parser, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(false, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(null, unique, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, Parsing.Parser parser, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(null, unique, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(indexName, false, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, Parsing.Parser parser, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(indexName, false, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(indexName, unique, false, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, Parsing.Parser parser, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(indexName, unique, false, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(null, unique, clustered, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, Parsing.Parser parser, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(null, unique, clustered, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, params IndexColumn<TTable>[] columns)
            => Database.AddIndex(indexName, unique, clustered, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, Parsing.Parser parser, params IndexColumn<TTable>[] columns)
        {
            var context = parser.ParseTableType(typeof(TTable));
            var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                              .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
            Database.AddIndex<TTable>(indexName, unique, clustered, parser, cols.ToArray());
        }


        static public void AddIndex<TTable>(this ITransformationProvider Database, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(false, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, Parsing.Parser parser, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(false, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(null, unique, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, Parsing.Parser parser, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(null, unique, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(indexName, false, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, Parsing.Parser parser, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(indexName, false, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(indexName, unique, false, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, Parsing.Parser parser, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(indexName, unique, false, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(null, unique, clustered, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, bool unique, bool clustered, Parsing.Parser parser, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(null, unique, clustered, parser, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, params IndexColumn[] columns)
            => Database.AddIndex<TTable>(indexName, unique, clustered, Parsing.Parser.Default, columns);

        static public void AddIndex<TTable>(this ITransformationProvider Database, string indexName, bool unique, bool clustered, Parsing.Parser parser, params IndexColumn[] columns)
            => Database.AddIndex(parser.ParseTableType(typeof(TTable)).Table.Name, indexName, unique, clustered, parser, columns.Select(c => c.ColumnNameWithDirection).ToArray());


        static public void AddIndex(this ITransformationProvider Database, string table, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, false, Parsing.Parser.Default, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, Parsing.Parser parser, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, false, parser, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, bool unique, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, null, unique, Parsing.Parser.Default, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, bool unique, Parsing.Parser parser, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, null, unique, parser, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, indexName, false, Parsing.Parser.Default, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, Parsing.Parser parser, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, indexName, false, parser, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, bool unique, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, indexName, unique, false, Parsing.Parser.Default, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, bool unique, Parsing.Parser parser, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, indexName, unique, false, parser, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, bool unique, bool clustered, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, null, unique, clustered, Parsing.Parser.Default, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, bool unique, bool clustered, Parsing.Parser parser, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, null, unique, clustered, parser, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, bool unique, bool clustered, params string[] columnNamesWithDirection)
            => Database.AddIndex(table, indexName, unique, clustered, Parsing.Parser.Default, columnNamesWithDirection);

        static public void AddIndex(this ITransformationProvider Database, string table, string indexName, bool unique, bool clustered, Parsing.Parser parser, params string[] columnNamesWithDirection)
            => Database.ExecuteNonQuery($"CREATE {(unique ? "UNIQUE " : "")}{(clustered ? "CLUSTERED" : "NONCLUSTERED")} INDEX {indexName ?? parser.Conventions.IndexNameByTableAndColumnNames(table, RemoveDirection(columnNamesWithDirection))} ON [{table}] ({string.Join(", ", columnNamesWithDirection)})");

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


        static public void RemoveIndex<TTable>(this ITransformationProvider Database, params Expression<Func<TTable, object>>[] columns)
            => Database.RemoveIndex(Parsing.Parser.Default, columns);

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
            => Database.RemoveIndex(parser, columns.Select(c => new IndexColumn<TTable>(c)).ToArray());

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, params IndexColumn<TTable>[] columns)
            => Database.RemoveIndex(Parsing.Parser.Default, columns);

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, Parsing.Parser parser, params IndexColumn<TTable>[] columns)
        {
            var context = parser.ParseTableType(typeof(TTable));
            var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                              .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
            Database.RemoveIndex<TTable>(parser, cols.ToArray());
        }

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, params IndexColumn[] columns)
            => Database.RemoveIndex<TTable>(Parsing.Parser.Default, columns);

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, Parsing.Parser parser, params IndexColumn[] columns)
            => Database.RemoveIndex<TTable>(parser, columns.Select(c => c.ColumnName).ToArray());

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, params string[] columns)
            => Database.RemoveIndex<TTable>(Parsing.Parser.Default, columns);

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, Parsing.Parser parser, params string[] columns)
            => Database.RemoveIndex(parser.ParseTableType(typeof(TTable)).Table.Name, parser, columns);

        static public void RemoveIndex(this ITransformationProvider Database, string table, params string[] columns)
            => Database.RemoveIndex(table, Parsing.Parser.Default, columns);

        static public void RemoveIndex(this ITransformationProvider Database, string table, Parsing.Parser parser, params string[] columns)
            => Database.RemoveIndex(table, parser.Conventions.IndexNameByTableAndColumnNames(table, columns));

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, string indexName)
            => Database.RemoveIndex<TTable>(Parsing.Parser.Default, indexName);

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, Parsing.Parser parser, string indexName)
            => Database.RemoveIndex(parser.ParseTableType(typeof(TTable)).Table.Name, indexName);

        static public void RemoveIndex(this ITransformationProvider Database, string table, string indexName)
            => Database.ExecuteNonQuery($"DROP INDEX {indexName} ON [{table}]");
    }
}
