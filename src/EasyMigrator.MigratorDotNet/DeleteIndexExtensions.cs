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
            => Database.RemoveIndex(columns.Select(c => new IndexColumn<TTable>(c)).ToArray());

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, params IndexColumn<TTable>[] columns)
        {
            var context = typeof(TTable).ParseTable();
            var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                              .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
            Database.RemoveIndex<TTable>(cols.ToArray());
        }

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, params IndexColumn[] columns)
            => Database.RemoveIndex<TTable>(columns.Select(c => c.ColumnName).ToArray());

        static public void RemoveIndex<TTable>(this ITransformationProvider Database, params string[] columns)
            => Database.RemoveIndex(typeof(TTable).ParseTable().Table.Name, columns);

        static public void RemoveIndex(this ITransformationProvider Database, string table, params string[] columns)
            => Database.RemoveIndexByName(table, Parsing.Parser.Current.Conventions.IndexNameByTableAndColumnNames(table, columns));

        static public void RemoveIndexByName<TTable>(this ITransformationProvider Database, string indexName)
            => Database.RemoveIndexByName(typeof(TTable).ParseTable().Table.Name, indexName);

        static public void RemoveIndexByName(this ITransformationProvider Database, string table, string indexName)
            => Database.ExecuteNonQuery($"DROP INDEX {indexName.SqlQuote()} ON {table.SqlQuote()}");
    }
}
