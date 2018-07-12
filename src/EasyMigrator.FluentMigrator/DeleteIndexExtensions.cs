using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EasyMigrator.Extensions;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Delete.Index;


namespace EasyMigrator
{
    static public class DeleteIndexExtensions
    {
        static public IDeleteIndexOnColumnSyntax<TTable> Index<TTable>(this IDeleteExpressionRoot Delete)
            => new NamedDeleteIndexOnColumnSyntax<TTable>(Delete);

        static public IDeleteIndexOnColumnSyntax Index(this IDeleteExpressionRoot Delete, Type tableType)
            => new NamedDeleteIndexOnColumnSyntax(Delete, tableType);


        static public void OnTable<TTable>(this IDeleteIndexForTableSyntax DeleteIndex)
            => DeleteIndex.OnTable(typeof(TTable));

        static public void OnTable(this IDeleteIndexForTableSyntax DeleteIndex, Type tableType)
            => DeleteIndex.OnTable(tableType.ParseTable().Table.Name);

        public interface IDeleteIndexOnColumnSyntax
        {
            void OnColumns<TTable>(params Expression<Func<TTable, object>>[] columns);
            void OnColumns<TTable>(params IndexColumn<TTable>[] columns);
            void OnColumns(params IndexColumn[] columns);
        }

        public interface IDeleteIndexOnColumnSyntax<TTable>
        {
            void OnColumns(params Expression<Func<TTable, object>>[] columns);
            void OnColumns(params IndexColumn<TTable>[] columns);
            void OnColumns(params IndexColumn[] columns);
        }


        private class NamedDeleteIndexOnColumnSyntax : IDeleteIndexOnColumnSyntax
        {
            private readonly IDeleteExpressionRoot _create;
            private readonly Type _tableType;

            public NamedDeleteIndexOnColumnSyntax(IDeleteExpressionRoot Delete, Type tableType)
            {
                _create = Delete;
                _tableType = tableType;
            }

            public void OnColumns(params IndexColumn[] columns)
            {
                var context = _tableType.ParseTable();
                _create.Index(context.Conventions.IndexNameByTableAndColumnNames(context.Table.Name, columns.Select(c => c.ColumnName)))
                        .OnTable(context.Table.Name);
            }

            public void OnColumns<TTable>(params Expression<Func<TTable, object>>[] columns)
                => OnColumns(columns.Select(c => new IndexColumn<TTable>(c)).ToArray());

            public void OnColumns<TTable>(params IndexColumn<TTable>[] columns)
            {
                var context = typeof(TTable).ParseTable();
                var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                                  .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
                OnColumns(cols.ToArray());
            }
        }

        private class NamedDeleteIndexOnColumnSyntax<TTable> : IDeleteIndexOnColumnSyntax<TTable>
        {
            private readonly IDeleteExpressionRoot _create;

            public NamedDeleteIndexOnColumnSyntax(IDeleteExpressionRoot Delete)
            {
                _create = Delete;
            }

            public void OnColumns(params IndexColumn[] columns)
            {
                var context = typeof(TTable).ParseTable();
                _create.Index(context.Conventions.IndexNameByTableAndColumnNames(context.Table.Name, columns.Select(c => c.ColumnName)))
                        .OnTable(context.Table.Name);
            }

            public void OnColumns(params Expression<Func<TTable, object>>[] columns)
                => OnColumns(columns.Select(c => new IndexColumn<TTable>(c)).ToArray());

            public void OnColumns(params IndexColumn<TTable>[] columns)
            {
                var context = typeof(TTable).ParseTable();
                var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                                  .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
                OnColumns(cols.ToArray());
            }
        }
    }

}
