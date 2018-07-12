using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EasyMigrator.Extensions;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Index;


namespace EasyMigrator
{
    static public class CreateIndexExtensions
    {
        static public ICreateIndexOnColumnSyntax<TTable> Index<TTable>(this ICreateExpressionRoot Create)
            => new NamedCreateIndexOnColumnSyntax<TTable>(Create);

        static public ICreateIndexOnColumnSyntax Index(this ICreateExpressionRoot Create, Type tableType)
            => new NamedCreateIndexOnColumnSyntax(Create, tableType);

        static public ICreateIndexOnColumnSyntax<TTable> OnTable<TTable>(this ICreateIndexForTableSyntax CreateIndex)
            => new CreateIndexOnColumnSyntax<TTable>(CreateIndex.OnTable(typeof(TTable).ParseTable().Table.Name));

        static public ICreateIndexOnColumnSyntax OnTable(this ICreateIndexForTableSyntax CreateIndex, Type tableType)
            => new CreateIndexOnColumnSyntax(CreateIndex.OnTable(tableType.ParseTable().Table.Name));

        public interface ICreateIndexOptionsSyntax
        {
            FluentMigrator.Builders.Create.Index.ICreateIndexOptionsSyntax WithOptions();
        }

        public interface ICreateIndexOnColumnSyntax
        {
            ICreateIndexOptionsSyntax OnColumns<TTable>(params Expression<Func<TTable, object>>[] columns);
            ICreateIndexOptionsSyntax OnColumns<TTable>(params IndexColumn<TTable>[] columns);
            ICreateIndexOptionsSyntax OnColumns(params IndexColumn[] columns);
        }

        public interface ICreateIndexOnColumnSyntax<TTable>
        {
            ICreateIndexOptionsSyntax OnColumns(params Expression<Func<TTable, object>>[] columns);
            ICreateIndexOptionsSyntax OnColumns(params IndexColumn<TTable>[] columns);
            ICreateIndexOptionsSyntax OnColumns(params IndexColumn[] columns);
        }


        private class CreateIndexOptionsSyntax : ICreateIndexOptionsSyntax
        {
            private readonly FluentMigrator.Builders.Create.Index.ICreateIndexOnColumnSyntax _createIndexOnColumnSyntax;
            public CreateIndexOptionsSyntax(FluentMigrator.Builders.Create.Index.ICreateIndexOnColumnSyntax createIndexOnColumnSyntax) { _createIndexOnColumnSyntax = createIndexOnColumnSyntax; }
            public FluentMigrator.Builders.Create.Index.ICreateIndexOptionsSyntax WithOptions() => _createIndexOnColumnSyntax.WithOptions();
        }

        private class CreateIndexOnColumnSyntaxBase
        {
            protected FluentMigrator.Builders.Create.Index.ICreateIndexOnColumnSyntax CreateIndexOnColumnSyntax { get; set; }

            public CreateIndexOnColumnSyntaxBase() { }
            public CreateIndexOnColumnSyntaxBase(FluentMigrator.Builders.Create.Index.ICreateIndexOnColumnSyntax createIndexOnColumnSyntax) { CreateIndexOnColumnSyntax = createIndexOnColumnSyntax; }

            public virtual ICreateIndexOptionsSyntax OnColumns(params IndexColumn[] columns)
            {
                var idx = CreateIndexOnColumnSyntax;

                foreach (var col in columns) {
                    var cx = idx.OnColumn(col.ColumnName);
                    if (col.Direction == SortOrder.Descending)
                        idx = cx.Descending();
                    else if (col.Direction == SortOrder.Ascending || columns.Length > 1)
                        idx = cx.Ascending();
                }
                
                return new CreateIndexOptionsSyntax(idx);
            }
        }

        private class NamedCreateIndexOnColumnSyntax : CreateIndexOnColumnSyntax
        {
            private readonly ICreateExpressionRoot _create;
            private readonly Type _tableType;

            public NamedCreateIndexOnColumnSyntax(ICreateExpressionRoot Create, Type tableType)
            {
                _create = Create;
                _tableType = tableType;
            }

            public override ICreateIndexOptionsSyntax OnColumns(params IndexColumn[] columns)
            {
                var context = _tableType.ParseTable();
                CreateIndexOnColumnSyntax = 
                    _create.Index(context.Conventions.IndexNameByTableAndColumnNames(context.Table.Name, columns.Select(c => c.ColumnName)))
                           .OnTable(context.Table.Name);
                return base.OnColumns(columns);
            }
        }

        private class NamedCreateIndexOnColumnSyntax<TTable> : CreateIndexOnColumnSyntax<TTable>
        {
            private readonly ICreateExpressionRoot _create;

            public NamedCreateIndexOnColumnSyntax(ICreateExpressionRoot Create)
            {
                _create = Create;
            }

            public override ICreateIndexOptionsSyntax OnColumns(params IndexColumn[] columns)
            {
                var context = typeof(TTable).ParseTable();
                CreateIndexOnColumnSyntax =
                    _create.Index(context.Conventions.IndexNameByTableAndColumnNames(context.Table.Name, columns.Select(c => c.ColumnName)))
                           .OnTable(context.Table.Name);
                return base.OnColumns(columns);
            }
        }

        private class CreateIndexOnColumnSyntax : CreateIndexOnColumnSyntaxBase, ICreateIndexOnColumnSyntax
        {
            public CreateIndexOnColumnSyntax() { }
            public CreateIndexOnColumnSyntax(FluentMigrator.Builders.Create.Index.ICreateIndexOnColumnSyntax createIndexOnColumnSyntax) : base(createIndexOnColumnSyntax) { }

            public ICreateIndexOptionsSyntax OnColumns<TTable>(params Expression<Func<TTable, object>>[] columns)
                => OnColumns(columns.Select(c => new IndexColumn<TTable>(c)).ToArray());

            public ICreateIndexOptionsSyntax OnColumns<TTable>(params IndexColumn<TTable>[] columns)
            {
                var context = typeof(TTable).ParseTable();
                var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                                  .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
                return OnColumns(cols.ToArray());
            }
        }

        private class CreateIndexOnColumnSyntax<TTable> : CreateIndexOnColumnSyntaxBase, ICreateIndexOnColumnSyntax<TTable>
        {
            public CreateIndexOnColumnSyntax() { }
            public CreateIndexOnColumnSyntax(FluentMigrator.Builders.Create.Index.ICreateIndexOnColumnSyntax createIndexOnColumnSyntax) : base(createIndexOnColumnSyntax) { }

            public ICreateIndexOptionsSyntax OnColumns(params Expression<Func<TTable, object>>[] columns)
                => OnColumns(columns.Select(c => new IndexColumn<TTable>(c)).ToArray());

            public ICreateIndexOptionsSyntax OnColumns(params IndexColumn<TTable>[] columns)
            {
                var context = typeof(TTable).ParseTable();
                var cols = columns.Select(c => new { c, fi = c.ColumnExpression.GetExpressionField() })
                                  .Select(o => new IndexColumn(context.Columns[o.fi].Name, o.c.Direction));
                return OnColumns(cols.ToArray());
            }
        }
    }

}
