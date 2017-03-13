using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EasyMigrator.Extensions;
using Migrator.Framework;


namespace EasyMigrator.MigratorDotNet
{
    static public class PrimaryKeyExtensions
    {
        static public void AddPrimaryKey<TTable>(this ITransformationProvider db, params Expression<Func<TTable, object>>[] columns) => db.AddPrimaryKey(true, Parsing.Parser.Default, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider db, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns) => db.AddPrimaryKey(true, parser, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider db, bool clustered, params Expression<Func<TTable, object>>[] columns) => db.AddPrimaryKey(null, clustered, Parsing.Parser.Default, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider db, bool clustered, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns) => db.AddPrimaryKey(null, clustered, parser, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider db, string constraintName, bool clustered, params Expression<Func<TTable, object>>[] columns) => db.AddPrimaryKey(constraintName, clustered, Parsing.Parser.Default, columns);
        static public void AddPrimaryKey<TTable>(this ITransformationProvider db, string constraintName, bool clustered, Parsing.Parser parser, params Expression<Func<TTable, object>>[] columns)
        {
            var context = parser.ParseTableType(typeof(TTable));
            var colNames = columns.Select(e => e.GetExpressionField()).Select(fi => context.Columns[fi].Name);
            db.AddPrimaryKey(context.Table.Name, constraintName ?? context.Table.PrimaryKeyName, clustered, colNames.ToArray());
        }
        static public void AddPrimaryKey(this ITransformationProvider db, string table, params string[] columns) => db.AddPrimaryKey(table, true, columns);
        static public void AddPrimaryKey(this ITransformationProvider db, string table, bool clustered, params string[] columns) => db.AddPrimaryKey(table, null, clustered, columns);
        static public void AddPrimaryKey(this ITransformationProvider db, string table, string constraintName, bool clustered, params string[] columns)

            => db.ExecuteNonQuery($"ALTER TABLE [{table}] ADD CONSTRAINT {(constraintName ?? $"PK_{table}")} PRIMARY KEY {(clustered ? "CLUSTERED" : "NONCLUSTERED")} ({string.Join(", ", columns)})");
    }
}
