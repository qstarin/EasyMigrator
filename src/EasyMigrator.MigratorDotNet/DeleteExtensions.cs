using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Extensions;
using EasyMigrator.Parsing.Model;
using Migrator.Framework;


namespace EasyMigrator
{
    static public class DeleteExtensions
    {
        static public void RemoveTable<T>(this ITransformationProvider Database) => Database.RemoveTable(typeof(T));
        static public void RemoveTable(this ITransformationProvider Database, Type tableType)
        {
            var table = tableType.ParseTable().Table;
            Database.RemoveForeignKeys(table);
            Database.RemoveIndices(table);
            Database.RemoveTable(table.Name.SqlQuote());
        }

        static public void RemoveColumns<T>(this ITransformationProvider Database) => Database.RemoveColumns(typeof(T));
        static public void RemoveColumns(this ITransformationProvider Database, Type tableType)
        {
            var table = tableType.ParseTable().Table;
            Database.RemoveForeignKeys(table);
            Database.RemoveIndices(table);
            Database.RemoveColumns(table);
        }

        static private void RemoveColumns(this ITransformationProvider Database, Table table)
        {
            foreach (var c in table.Columns.DefinedInPoco())
                Database.RemoveColumn(table.Name, c.Name);
        }

        static private void RemoveForeignKeys(this ITransformationProvider Database, Table table)
        {
            foreach (var c in table.Columns.DefinedInPoco()) {
                var f = c.ForeignKey;
                if (f != null)
                    Database.RemoveForeignKey(table.Name, f.Name);
            }
        }

        static private void RemoveIndices(this ITransformationProvider Database, Table table)
        {
            foreach (var ci in table.Indices)
                Database.RemoveIndexByName(table.Name, ci.Name);
        }
    }
}
