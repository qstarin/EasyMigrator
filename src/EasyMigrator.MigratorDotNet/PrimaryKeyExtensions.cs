using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EasyMigrator.Extensions;
using Migrator.Framework;


namespace EasyMigrator
{
    static public class PrimaryKeyExtensions
    {
        static public void AddPrimaryKey<TTable>(this ITransformationProvider Database, params Expression<Func<TTable, object>>[] columns) => Database.AddPrimaryKey(true, Parsing.Parser.Default, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider Database, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns) => Database.AddPrimaryKey(true, parser, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider Database, bool clustered, params Expression<Func<TTable, object>>[] columns) => Database.AddPrimaryKey(null, clustered, Parsing.Parser.Default, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider Database, bool clustered, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns) => Database.AddPrimaryKey(null, clustered, parser, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider Database, string constraintName, bool clustered, params Expression<Func<TTable, object>>[] columns) => Database.AddPrimaryKey(constraintName, clustered, Parsing.Parser.Default, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider Database, string constraintName, bool clustered, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
        {
            var context = parser.ParseTableType(typeof(TTable));
            var colNames = columns.Select(e => e.GetExpressionField()).Select(fi => SqlReservedWords.Quote(context.Columns[fi].Name));
            Database.AddPrimaryKey(SqlReservedWords.Quote(context.Table.Name), constraintName ?? context.Table.PrimaryKeyName, clustered, colNames.ToArray());
        }
        static public void AddPrimaryKey(this ITransformationProvider Database, string table, params string[] columns) => Database.AddPrimaryKey(table, true, columns);
        static public void AddPrimaryKey(this ITransformationProvider Database, string table, bool clustered, params string[] columns) => Database.AddPrimaryKey(table, null, clustered, columns);
        static public void AddPrimaryKey(this ITransformationProvider Database, string table, string constraintName, bool clustered, params string[] columns)

            => Database.ExecuteNonQuery($"ALTER TABLE {table} ADD CONSTRAINT {constraintName ?? Parsing.Parser.Default.Conventions.PrimaryKeyNameByTableName(table)} PRIMARY KEY {(clustered ? "CLUSTERED" : "NONCLUSTERED")} ({string.Join(", ", columns)})");
    }
}
