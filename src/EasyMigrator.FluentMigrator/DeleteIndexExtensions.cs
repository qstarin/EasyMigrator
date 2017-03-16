using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            => Delete.Index<TTable>(Parsing.Parser.Default);

        static public IDeleteIndexOnColumnSyntax<TTable> Index<TTable>(this IDeleteExpressionRoot Delete, Parsing.Parser parser)
            => new NamedDeleteIndexOnColumnSyntax<TTable>(Delete, parser);

        static public IDeleteIndexOnColumnSyntax Index(this IDeleteExpressionRoot Delete, Type tableType)
            => Delete.Index(tableType, Parsing.Parser.Default);

        static public IDeleteIndexOnColumnSyntax Index(this IDeleteExpressionRoot Delete, Type tableType, Parsing.Parser parser)
            => new NamedDeleteIndexOnColumnSyntax(Delete, tableType, parser);


        static public void OnTable<TTable>(this IDeleteIndexForTableSyntax DeleteIndex)
            => DeleteIndex.OnTable<TTable>(Parsing.Parser.Default);

        static public void OnTable<TTable>(this IDeleteIndexForTableSyntax DeleteIndex, Parsing.Parser parser)
            => DeleteIndex.OnTable(typeof(TTable), parser);

        static public void OnTable(this IDeleteIndexForTableSyntax DeleteIndex, Type tableType)
            => DeleteIndex.OnTable(tableType, Parsing.Parser.Default);

        static public void OnTable(this IDeleteIndexForTableSyntax DeleteIndex, Type tableType, Parsing.Parser parser)
            => DeleteIndex.OnTable(parser.ParseTableType(tableType).Table.Name);


        private class NamedDeleteIndexOnColumnSyntax : IDeleteIndexOnColumnSyntax
        {
            private readonly IDeleteExpressionRoot _create;
            private readonly Type _tableType;
            private readonly Parsing.Parser _parser;

            public NamedDeleteIndexOnColumnSyntax(IDeleteExpressionRoot Delete, Type tableType, Parsing.Parser parser)
            {
                _create = Delete;
                _tableType = tableType;
                _parser = parser;
            }

            public void OnColumns(params IndexColumn[] columns)
            {
                var context = _parser.ParseTableType(_tableType);
                _create.Index(_parser.Conventions.IndexNameByTableAndColumnNames(context.Table.Name, columns.Select(c => c.ColumnName)))
                        .OnTable(context.Table.Name);
            }

            public void OnColumns<TTable>(params Expression<Func<TTable, object>>[] columns) => OnColumns(Parsing.Parser.Default, columns);
            public void OnColumns<TTable>(Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
                => OnColumns(Parsing.Parser.Default, columns.Select(c => new IndexColumn<TTable>(c)).ToArray());

            public void OnColumns<TTable>(params IndexColumn<TTable>[] columns) => OnColumns(Parsing.Parser.Default, columns);
            public void OnColumns<TTable>(Parsing.Parser parser, params IndexColumn<TTable>[] columns)
            {
                var context = parser.ParseTableType(typeof(TTable));
                var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                                  .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
                OnColumns(cols.ToArray());
            }
        }

        private class NamedDeleteIndexOnColumnSyntax<TTable> : IDeleteIndexOnColumnSyntax<TTable>
        {
            private readonly IDeleteExpressionRoot _create;
            private readonly Parsing.Parser _parser;

            public NamedDeleteIndexOnColumnSyntax(IDeleteExpressionRoot Delete, Parsing.Parser parser)
            {
                _create = Delete;
                _parser = parser;
            }

            public void OnColumns(params IndexColumn[] columns)
            {
                var context = _parser.ParseTableType(typeof(TTable));
                _create.Index(_parser.Conventions.IndexNameByTableAndColumnNames(context.Table.Name, columns.Select(c => c.ColumnName)))
                        .OnTable(context.Table.Name);
            }

            public void OnColumns(params Expression<Func<TTable, object>>[] columns) => OnColumns(Parsing.Parser.Default, columns);
            public void OnColumns(Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
                => OnColumns(Parsing.Parser.Default, columns.Select(c => new IndexColumn<TTable>(c)).ToArray());

            public void OnColumns(params IndexColumn<TTable>[] columns) => OnColumns(Parsing.Parser.Default, columns);
            public void OnColumns(Parsing.Parser parser, params IndexColumn<TTable>[] columns)
            {
                var context = parser.ParseTableType(typeof(TTable));
                var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                                  .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
                OnColumns(cols.ToArray());
            }
        }
    }

    public interface IDeleteIndexOnColumnSyntax
    {
        void OnColumns<TTable>(params Expression<Func<TTable, object>>[] columns);
        void OnColumns<TTable>(Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns);
        void OnColumns<TTable>(params IndexColumn<TTable>[] columns);
        void OnColumns<TTable>(Parsing.Parser parser, params IndexColumn<TTable>[] columns);
        void OnColumns(params IndexColumn[] columns);
    }

    public interface IDeleteIndexOnColumnSyntax<TTable>
    {
        void OnColumns(params Expression<Func<TTable, object>>[] columns);
        void OnColumns(Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns);
        void OnColumns(params IndexColumn<TTable>[] columns);
        void OnColumns(Parsing.Parser parser, params IndexColumn<TTable>[] columns);
        void OnColumns(params IndexColumn[] columns);
    }

}
