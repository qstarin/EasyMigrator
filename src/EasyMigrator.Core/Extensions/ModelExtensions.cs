using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Extensions
{
    static public class ModelExtensions
    {
        static public Parsing.Context ParseTable(this Type tableType) => Parsing.Parser.Current.ParseTableType(tableType);
        static public IEnumerable<Column> DefinedInPoco(this IEnumerable<Column> columns) => columns.Where(c => c.DefinedInPoco);
        static public IEnumerable<Column> PrimaryKey(this IEnumerable<Column> columns) => columns.Where(c => c.IsPrimaryKey);
        static public IEnumerable<Column> ForeignKeys(this IEnumerable<Column> columns) => columns.Where(c => c.ForeignKey != null);
        static public IEnumerable<Column> MaxLength(this IEnumerable<Column> columns) => columns.Where(c => (c.Type == DbType.AnsiString || c.Type == DbType.String) && c.Length == int.MaxValue);
        static public IEnumerable<Column> WithPrecision(this IEnumerable<Column> columns) => columns.Where(c => c.Precision != null);
        static public IEnumerable<Column> WithCustomAutoIncrement(this IEnumerable<Column> columns) => columns.Where(c => c.IsCustomAutoIncrement());
        static public IEnumerable<Column> WithoutCustomAutoIncrement(this IEnumerable<Column> columns) => columns.Where(c => !c.IsCustomAutoIncrement());
        static public bool IsDefaultAutoIncrement(this Column column) => column.AutoIncrement?.Seed == 1 && column.AutoIncrement?.Step == 1;
        static public bool IsCustomAutoIncrement(this Column column) => column.AutoIncrement != null && column.AutoIncrement.Seed != 1 && column.AutoIncrement.Step != 1;
    }
}
