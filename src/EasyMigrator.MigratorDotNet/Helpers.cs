using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.MigratorDotNet
{
    // TODO: This should probably be part of the conventions and used across all migrators..
    static public class Helpers
    {
        static public string BuildForeignKeyName(string foreignTable, string foreignColumn) => $"FK_{foreignTable}_{foreignColumn}";
        static public string BuildIndexName(string table, params string[] columns) => $"IX_{table}_{string.Join("_", columns)}";
    }
}
