﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Model;
using EasyMigrator.Extensions;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Runner.Extensions;


namespace EasyMigrator
{
    static public class CreateExtensions
    {
        static public void Table<T>(this IDeleteExpressionRoot Delete) { Delete.Table(typeof(T)); }
        static public void Table(this IDeleteExpressionRoot Delete, Type tableType) { Delete.Table(tableType, Parsing.Parser.Default); }
        public static void Table<T>(this IDeleteExpressionRoot Delete, Parsing.Parser parser) { Delete.Table(typeof(T), parser); }
        static public void Table(this IDeleteExpressionRoot Delete, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTable(tableType);
            table.Columns.ForEach(c => c.ForeignKey.IfNotNull(f => {
                if (f.Name != null)
                    Delete.ForeignKey(f.Name).OnTable(table.Name);
                else 
                    Delete.ForeignKey().FromTable(table.Name).ForeignColumn(c.Name).ToTable(f.Table).PrimaryColumn(f.Column);
            }));
            Delete.Table(table.Name);
        }

        static public void Table<T>(this ICreateExpressionRoot Create) { Create.Table(typeof(T)); }
        static public void Table(this ICreateExpressionRoot Create, Type tableType) { Create.Table(tableType, Parsing.Parser.Default); }
        public static void Table<T>(this ICreateExpressionRoot Create, Parsing.Parser parser) { Create.Table(typeof(T), parser); }
        static public void Table(this ICreateExpressionRoot Create, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTable(tableType);
            var createTableSyntax = Create.Table(table.Name);
            foreach (var col in table.Columns) {
                var createColumnOptionSyntax = createTableSyntax.WithColumn(col.Name).As(col);

                createColumnOptionSyntax = col.IsNullable
                                               ? createColumnOptionSyntax.Nullable()
                                               : createColumnOptionSyntax.NotNullable();

                if (col.IsPrimaryKey)
                    createColumnOptionSyntax = createColumnOptionSyntax.PrimaryKey();

                if (col.ForeignKey != null)
                    createColumnOptionSyntax = createColumnOptionSyntax.ForeignKey(col.ForeignKey.Table, col.ForeignKey.Column);

                if (col.DefaultValue != null)
                    createColumnOptionSyntax = createColumnOptionSyntax.WithDefaultValue(col.DefaultValue);

                if (col.AutoIncrement != null) 
                    createColumnOptionSyntax = createColumnOptionSyntax.Identity((int)col.AutoIncrement.Seed, (int)col.AutoIncrement.Step);

                if (col.Index != null) {
                    createColumnOptionSyntax = 
                        col.Index.Unique
                            ? string.IsNullOrEmpty(col.Index.Name) ? createColumnOptionSyntax.Unique() : createColumnOptionSyntax.Unique(col.Index.Name)
                            : string.IsNullOrEmpty(col.Index.Name) ? createColumnOptionSyntax.Indexed() : createColumnOptionSyntax.Indexed(col.Index.Name);
                }
            }
        }

        
        static private ICreateTableColumnOptionOrWithColumnSyntax As(this ICreateTableColumnAsTypeSyntax s, Column col)
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
