using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Migrator.Framework;


namespace EasyMigrator.MigratorDotNet
{
    static public class IdentityInsertExtensions
    {

        static public IDisposable WithIdentityInsert<T>(this ITransformationProvider Database) => Database.WithIdentityInsert(typeof(T));
        static public IDisposable WithIdentityInsert(this ITransformationProvider Database, Type tableType) => Database.WithIdentityInsert(tableType, Parsing.Parser.Default);
        static public IDisposable WithIdentityInsert<T>(this ITransformationProvider Database, Parsing.Parser parser) => Database.WithIdentityInsert(typeof(T), parser);
        static public IDisposable WithIdentityInsert(this ITransformationProvider Database, Type tableType, Parsing.Parser parser) => Database.WithIdentityInsert(parser.ParseTableType(tableType).Table.Name);
        static public IDisposable WithIdentityInsert(this ITransformationProvider Database, string table) => new IdentityInsertScope(Database, table);

        private class IdentityInsertScope : IDisposable
        {
            private readonly string _table;
            private readonly ITransformationProvider _database;

            public IdentityInsertScope(ITransformationProvider database, string table)
            {
                _table = table;
                _database = database;
                SetOn();
            }

            private void SetOn() => _database.ExecuteNonQuery("SET IDENTITY_INSERT " + _table + " ON");
            private void SetOff() => _database.ExecuteNonQuery("SET IDENTITY_INSERT " + _table + " OFF");
            public void Dispose() => SetOff();
        }
    }
}
