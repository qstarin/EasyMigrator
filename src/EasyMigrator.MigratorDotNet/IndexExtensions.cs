using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Migrator.Framework;


namespace EasyMigrator.MigratorDotNet
{
    static public class IndexExtensions
    {
        static private void AddIndex(this ITransformationProvider Database, string table, string indexName, params string[] columnsWithDirection)
            => Database.AddIndex(table, indexName, false, false, columnsWithDirection);

        static private void AddIndex(this ITransformationProvider Database, string table, string indexName, bool unique, params string[] columnsWithDirection)
            => Database.AddIndex(table, indexName, unique, false, columnsWithDirection);

        static private void AddIndex(this ITransformationProvider Database, string table, string indexName, bool unique, bool clustered, params string[] columnsWithDirection)
            => Database.ExecuteNonQuery($"CREATE {(unique ? "UNIQUE " : "")}{(clustered ? "CLUSTERED" : "NONCLUSTERED")} INDEX {indexName} ON [{table}] ({string.Join(", ", columnsWithDirection)})");
    }
}
