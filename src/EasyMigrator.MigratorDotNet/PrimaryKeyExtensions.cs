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
        static public void AddPrimaryKey<TTable>(this ITransformationProvider Database, params Expression<Func<TTable, object>>[] columns) => Database.AddPrimaryKey(null, true, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider Database, bool clustered, params Expression<Func<TTable, object>>[] columns) => Database.AddPrimaryKey(null, clustered, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider Database, string constraintName, bool clustered, params Expression<Func<TTable, object>>[] columns)
        {
            var context = typeof(TTable).ParseTable();
            var colNames = columns.Select(e => e.GetExpressionField()).Select(fi => context.Columns[fi].Name);
            Database.AddPrimaryKey(context.Table.Name, constraintName ?? context.Table.PrimaryKeyName, clustered, colNames.ToArray());
        }

        static public void AddPrimaryKey(this ITransformationProvider Database, string table, params string[] columns) => Database.AddPrimaryKey(table, null, true, columns);
        static public void AddPrimaryKey(this ITransformationProvider Database, string table, bool clustered, params string[] columns) => Database.AddPrimaryKey(table, null, clustered, columns);
        static public void AddPrimaryKey(this ITransformationProvider Database, string table, string constraintName, bool clustered, params string[] columns)
            => Database.ExecuteNonQuery(
$"ALTER TABLE {table.SqlQuote()} " + 
$"ADD CONSTRAINT {(constraintName ?? Parsing.Parser.Current.Conventions.PrimaryKeyNameByTableName(table)).SqlQuote()} " +
$"PRIMARY KEY {(clustered ? "CLUSTERED" : "NONCLUSTERED")} " +
$"({string.Join(", ", columns.Select(c => c.SqlQuote()))})");

    }
}
