using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Index;


namespace EasyMigrator
{
    static public class CreateIndexExtensions
    {
        static public ICreateIndexOnColumnSyntax Index<TTable>(this ICreateExpressionRoot Create, params IndexColumn[] columns)
            => Create.Index(typeof(TTable), Parsing.Parser.Default, columns);

        static public ICreateIndexOnColumnSyntax Index<TTable>(this ICreateExpressionRoot Create, Parsing.Parser parser, params IndexColumn[] columns)
            => Create.Index(typeof(TTable), parser, columns);

        static public ICreateIndexOnColumnSyntax Index(this ICreateExpressionRoot Create, Type tableType, params IndexColumn[] columns)
            => Create.Index(tableType, Parsing.Parser.Default, columns);

        static public ICreateIndexOnColumnSyntax Index(this ICreateExpressionRoot Create, Type tableType, Parsing.Parser parser, params IndexColumn[] columns)
            => Create.Index(parser.Conventions.IndexNameByTableAndColumnNames(parser.ParseTableType(tableType).Table.Name, columns.Select(c => c.ColumnName)))
                     .OnTable(tableType, parser, columns);


        static public ICreateIndexOnColumnSyntax OnTable<TTable>(this ICreateIndexForTableSyntax CreateIndex, params IndexColumn[] columns)
            => CreateIndex.OnTable(typeof(TTable), Parsing.Parser.Default, columns);

        static public ICreateIndexOnColumnSyntax OnTable<TTable>(this ICreateIndexForTableSyntax CreateIndex, Parsing.Parser parser, params IndexColumn[] columns)
            => CreateIndex.OnTable(typeof(TTable), parser, columns);

        static public ICreateIndexOnColumnSyntax OnTable(this ICreateIndexForTableSyntax CreateIndex, Type tableType, params IndexColumn[] columns)
            => CreateIndex.OnTable(tableType, Parsing.Parser.Default, columns);

        static public ICreateIndexOnColumnSyntax OnTable(this ICreateIndexForTableSyntax CreateIndex, Type tableType, Parsing.Parser parser, params IndexColumn[] columns)
        {
            var context = parser.ParseTableType(tableType);
            ICreateIndexOnColumnSyntax idx = CreateIndex.OnTable(context.Table.Name);

            foreach (var col in columns) {
                var cx = idx.OnColumn(col.ColumnName);
                if (col.Direction == SortOrder.Descending)
                    idx = cx.Descending();
                else if (col.Direction == SortOrder.Ascending || columns.Length > 1)
                    idx = cx.Ascending();
            }

            return idx;
        }
    }
}
