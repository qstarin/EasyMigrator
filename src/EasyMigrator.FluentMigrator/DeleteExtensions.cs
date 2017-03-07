using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Extensions;
using EasyMigrator.Parsing.Model;
using FluentMigrator.Builders.Delete;


namespace EasyMigrator
{
    static public class DeleteExtensions
    {
        static public void Table<T>(this IDeleteExpressionRoot Delete) { Delete.Table(typeof(T)); }
        static public void Table(this IDeleteExpressionRoot Delete, Type tableType) { Delete.Table(tableType, Parsing.Parser.Default); }
        public static void Table<T>(this IDeleteExpressionRoot Delete, Parsing.Parser parser) { Delete.Table(typeof(T), parser); }
        static public void Table(this IDeleteExpressionRoot Delete, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTableType(tableType);
            Delete.ForeignKeys(table);
            Delete.Indexes(table);
            Delete.Table(table.Name);
        }

        static public void Columns<T>(this IDeleteExpressionRoot Delete) { Delete.Columns(typeof(T)); }
        static public void Columns(this IDeleteExpressionRoot Delete, Type tableType) { Delete.Columns(tableType, Parsing.Parser.Default); }
        public static void Columns<T>(this IDeleteExpressionRoot Delete, Parsing.Parser parser) { Delete.Columns(typeof(T), parser); }
        static public void Columns(this IDeleteExpressionRoot Delete, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTableType(tableType);
            Delete.ForeignKeys(table);
            Delete.Indexes(table);
            Delete.Columns(table);
        }

        static private void Columns(this IDeleteExpressionRoot Delete, Table table)
        {
            foreach (var c in table.Columns.DefinedInPoco())
                Delete.Column(c.Name).FromTable(table.Name);
        }

        static private void ForeignKeys(this IDeleteExpressionRoot Delete, Table table)
        {
            foreach (var c in table.Columns.DefinedInPoco()) {
                var f = c.ForeignKey;
                if (f == null)
                    continue;

                if (f.Name != null)
                    Delete.ForeignKey(f.Name).OnTable(table.Name);
                else
                    Delete.ForeignKey()
                          .FromTable(table.Name)
                          .ForeignColumn(c.Name)
                          .ToTable(f.Table)
                          .PrimaryColumn(f.Column);
            }
        }

        static private void Indexes(this IDeleteExpressionRoot Delete, Table table)
        {
            foreach (var c in table.Columns.DefinedInPoco()) {
                var i = c.Index;
                if (i == null)
                    continue;

                if (i.Name != null)
                    Delete.Index(i.Name).OnTable(table.Name);
                else
                    Delete.Index()
                          .OnTable(table.Name)
                          .OnColumn(c.Name);
            }
        }
    }
}
