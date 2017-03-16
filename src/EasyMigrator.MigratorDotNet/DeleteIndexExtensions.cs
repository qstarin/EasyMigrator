using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EasyMigrator.Extensions;
using Migrator.Framework;


namespace EasyMigrator
{
    static public class DeleteIndexExtensions
    {
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
            => Database.RemoveIndexByName(table, parser.Conventions.IndexNameByTableAndColumnNames(table, columns));

        static public void RemoveIndexByName<TTable>(this ITransformationProvider Database, string indexName)
            => Database.RemoveIndexByName<TTable>(Parsing.Parser.Default, indexName);

        static public void RemoveIndexByName<TTable>(this ITransformationProvider Database, Parsing.Parser parser, string indexName)
            => Database.RemoveIndexByName(parser.ParseTableType(typeof(TTable)).Table.Name, indexName);

        static public void RemoveIndexByName(this ITransformationProvider Database, string table, string indexName)
            => Database.ExecuteNonQuery($"DROP INDEX {indexName} ON [{table}]");
    }
}
