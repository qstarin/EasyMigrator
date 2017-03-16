using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;
using EasyMigrator.Extensions;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter;
using FluentMigrator.Builders.Alter.Column;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Column;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Runner.Extensions;


namespace EasyMigrator
{
    static public class CreateExtensions
    {
        static public void Table<T>(this ICreateExpressionRoot Create) => Create.Table(typeof(T));
        static public void Table(this ICreateExpressionRoot Create, Type tableType) => Create.Table(tableType, Parsing.Parser.Default);
        static public void Table<T>(this ICreateExpressionRoot Create, Parsing.Parser parser) => Create.Table(typeof(T), parser);
        static public void Table(this ICreateExpressionRoot Create, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTableType(tableType).Table;
            var createTableSyntax = Create.Table(table.Name);
            foreach (var col in table.Columns)
                createTableSyntax.WithColumn(col.Name)
                                 .BuildColumn<ICreateTableColumnAsTypeSyntax, 
                                              ICreateTableColumnOptionOrWithColumnSyntax,
                                              ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax>(table, col);

            var pkCreator = Create.PrimaryKey(table.PrimaryKeyName)
                                  .OnTable(table.Name)
                                  .Columns(table.Columns.Where(c => c.IsPrimaryKey)
                                  .Select(c => c.Name)
                                  .ToArray());

            if (table.PrimaryKeyIsClustered)
                pkCreator.Clustered();
            else
                pkCreator.NonClustered();

            foreach (var col in table.Columns.Where(c => c.Index?.Clustered ?? false))
                Create.UniqueConstraint(col.Index.Name).OnTable(table.Name).Columns(col.Name).Clustered();
        }

        static public void Columns<T>(this ICreateExpressionRoot Create) => Create.Columns(typeof(T));
        static public void Columns<T>(this ICreateExpressionRoot Create, IAlterExpressionRoot Alter, Action populate) => Create.Columns(typeof(T), Alter, populate);
        static public void Columns(this ICreateExpressionRoot Create, Type tableType) => Create.Columns(tableType, Parsing.Parser.Default);
        static public void Columns(this ICreateExpressionRoot Create, Type tableType, IAlterExpressionRoot Alter, Action populate) => Create.Columns(tableType, Parsing.Parser.Default, Alter, populate);
        static public void Columns<T>(this ICreateExpressionRoot Create, Parsing.Parser parser) => Create.Columns(typeof(T), parser);
        static public void Columns<T>(this ICreateExpressionRoot Create, Parsing.Parser parser, IAlterExpressionRoot Alter, Action populate) => Create.Columns(typeof(T), parser, Alter, populate);
        static public void Columns(this ICreateExpressionRoot Create, Type tableType, Parsing.Parser parser) => Create.Columns(tableType, parser, null, null);
        static public void Columns(this ICreateExpressionRoot Create, Type tableType, Parsing.Parser parser, IAlterExpressionRoot Alter, Action populate)
        {
            var table = parser.ParseTableType(tableType).Table;
            var nonNullables = new List<Column>();
            foreach (var col in table.Columns.DefinedInPoco()) { // avoids trying to add the default primary key column
                if (populate != null && !col.IsNullable && col.DefaultValue == null) {
                    col.IsNullable = true;
                    nonNullables.Add(col);
                }

                Create.Column(col.Name).OnTable(table.Name)
                                 .BuildColumn<ICreateColumnAsTypeOrInSchemaSyntax,
                                              ICreateColumnOptionSyntax,
                                              ICreateColumnOptionOrForeignKeyCascadeSyntax>(table, col);
            }

            foreach (var col in table.Columns.Where(c => c.Index?.Clustered ?? false))
                Create.UniqueConstraint(col.Index.Name).OnTable(table.Name).Columns(col.Name).Clustered();

            if (populate != null) {
                populate();
                foreach (var col in nonNullables) {
                    col.IsNullable = false;
                    Alter.Column(col.Name).OnTable(table.Name)
                                     .BuildColumn<IAlterColumnAsTypeOrInSchemaSyntax,
                                                  IAlterColumnOptionSyntax,
                                                  IAlterColumnOptionOrForeignKeyCascadeSyntax>(table, col);
                }
            }
        }

        static private void BuildColumn<TSyntax, TNext, TNextFk>(this TSyntax s, Table table, Column col)
            where TSyntax : IColumnTypeSyntax<TNext>
            where TNext : IColumnOptionSyntax<TNext, TNextFk>
            where TNextFk : IColumnOptionSyntax<TNext, TNextFk>, TNext
        {
            var createColumnOptionSyntax = s.As<TSyntax, TNext, TNextFk>(col);

            createColumnOptionSyntax = 
                col.IsNullable
                    ? createColumnOptionSyntax.Nullable()
                    : createColumnOptionSyntax.NotNullable();

            if (col.ForeignKey != null)
                createColumnOptionSyntax = createColumnOptionSyntax.ForeignKey(col.ForeignKey.Name, col.ForeignKey.Table, col.ForeignKey.Column);

            if (col.DefaultValue != null)
                createColumnOptionSyntax = createColumnOptionSyntax.WithDefaultValue(col.DefaultValue);

            if (col.AutoIncrement != null)
                createColumnOptionSyntax = createColumnOptionSyntax.Identity((int)col.AutoIncrement.Seed, (int)col.AutoIncrement.Step);

            if (col.Index != null && !col.Index.Clustered)
                createColumnOptionSyntax =
                    col.Index.Unique
                        ? createColumnOptionSyntax.Unique(col.Index.Name)
                        : createColumnOptionSyntax.Indexed(col.Index.Name);
        }

        static private TNext As<TSyntax, TNext, TNextFk>(this TSyntax s, Column col)
            where TSyntax : IColumnTypeSyntax<TNext>
            where TNext : IColumnOptionSyntax<TNext, TNextFk>
            where TNextFk : IColumnOptionSyntax<TNext, TNextFk>, TNext
        {
            switch (col.Type) {
                case DbType.AnsiString: return col.Length.IfHasValue(s.AsAnsiString, s.AsAnsiString);
                case DbType.AnsiStringFixedLength: return s.AsFixedLengthAnsiString(col.Length.Value);
                case DbType.Binary: return col.Length.IfHasValue(s.AsBinary, s.AsBinary);
                case DbType.Byte: return s.AsByte();
                case DbType.Boolean: return s.AsBoolean();
                case DbType.Currency: return s.AsCurrency();
                case DbType.Date: return s.AsDate();
                case DbType.DateTime: return s.AsDateTime();
                case DbType.DateTime2: return s.AsByte();
                //case DbType.DateTimeOffset: return s.AsDateTime(); // TODO: What can DateTimeOffset map this to?
                case DbType.Decimal: return col.Precision.IfNotNull(p => s.AsDecimal(p.Precision, p.Scale), s.AsDecimal);
                case DbType.Double: return s.AsDouble();
                case DbType.Guid: return s.AsGuid();
                case DbType.Int16: return s.AsInt16();
                case DbType.Int32: return s.AsInt32();
                case DbType.Int64: return s.AsInt64();
                //case DbType.Object: return s.AsByte(); // TODO: Use AsCustom
                case DbType.SByte: return s.AsByte(); // TODO: signed byte not supported directly by FluentMigrator
                case DbType.Single: return s.AsFloat();
                case DbType.String: return col.Length.IfHasValue(s.AsString, s.AsString);
                case DbType.StringFixedLength: return s.AsFixedLengthString(col.Length.Value);
                case DbType.Time: return s.AsTime();
                case DbType.UInt16: return s.AsInt16(); // TODO: unsigned integers not supported directly by FluentMigrator
                case DbType.UInt32: return s.AsInt32();
                case DbType.UInt64: return s.AsInt64();
                //case DbType.VarNumeric: return s.AsByte();
                case DbType.Xml: return col.Length.IfHasValue(s.AsXml, s.AsXml);
                default: throw new Exception("DbType '" + col.Type + "' is not mapped to a column type for FluentMigrator.");
            }
        }
    }
}
