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
        static public IEnumerable<Column> DefinedInPoco(this IEnumerable<Column> columns) => columns.Where(c => c.DefinedInPoco);
        static public IEnumerable<Column> PrimaryKey(this IEnumerable<Column> columns) => columns.Where(c => c.IsPrimaryKey);
        static public IEnumerable<Column> ForeignKeys(this IEnumerable<Column> columns) => columns.Where(c => c.ForeignKey != null);
        static public IEnumerable<Column> Indexed(this IEnumerable<Column> columns) => columns.Where(c => c.Index != null);
        static public IEnumerable<Column> MaxLength(this IEnumerable<Column> columns) => columns.Where(c => (c.Type == DbType.AnsiString || c.Type == DbType.String) && c.Length == int.MaxValue);
        static public IEnumerable<Column> WithPrecision(this IEnumerable<Column> columns) => columns.Where(c => (c.Type == DbType.Decimal || c.Type == DbType.Currency) && c.Precision != null);
    }
}
