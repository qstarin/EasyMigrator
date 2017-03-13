﻿using System;
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
        static public void RemoveTable<T>(this ITransformationProvider db) => db.RemoveTable(typeof(T));
        static public void RemoveTable(this ITransformationProvider db, Type tableType) => db.RemoveTable(tableType, Parsing.Parser.Default);
        static public void RemoveTable<T>(this ITransformationProvider db, Parsing.Parser parser) => db.RemoveTable(typeof(T), parser);
        static public void RemoveTable(this ITransformationProvider db, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTableType(tableType).Table;
            db.RemoveForeignKeys(table);
            db.RemoveIndexes(table);
            db.RemoveTable(table.Name);
        }

        static public void RemoveColumns<T>(this ITransformationProvider db) => db.RemoveColumns(typeof(T));
        static public void RemoveColumns(this ITransformationProvider db, Type tableType) => db.RemoveColumns(tableType, Parsing.Parser.Default);
        static public void RemoveColumns<T>(this ITransformationProvider db, Parsing.Parser parser) => db.RemoveColumns(typeof(T), parser);
        static public void RemoveColumns(this ITransformationProvider db, Type tableType, Parsing.Parser parser)
        {
            var table = parser.ParseTableType(tableType).Table;
            db.RemoveForeignKeys(table);
            db.RemoveIndexes(table);
            db.RemoveColumns(table);
        }

        static private void RemoveIndex(this ITransformationProvider db, string table, string name)
            => db.ExecuteNonQuery($"DROP INDEX {name} ON {table}");

        static private void RemoveColumns(this ITransformationProvider db, Table table)
        {
            foreach (var c in table.Columns.DefinedInPoco())
                db.RemoveColumn(table.Name, c.Name);
        }

        static private void RemoveForeignKeys(this ITransformationProvider db, Table table)
        {
            foreach (var c in table.Columns.DefinedInPoco()) {
                var f = c.ForeignKey;
                if (f != null)
                    db.RemoveForeignKey(table.Name, f.Name);
            }
        }

        static private void RemoveIndexes(this ITransformationProvider db, Table table)
        {
            foreach (var c in table.Columns.DefinedInPoco()) {
                var i = c.Index;
                if (i != null)
                    db.RemoveIndex(table.Name, i.Name);
            }
        }
    }
}
