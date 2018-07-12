using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Extensions;
using Migrator.Framework;


namespace EasyMigrator
{
    static public class IdentityInsertExtensions
    {
        static public IDisposable WithIdentityInsert<T>(this ITransformationProvider Database) => new IdentityInsertScope(Database, typeof(T).ParseTable().Table.Name);
        static public IDisposable WithIdentityInsert(this ITransformationProvider Database, Type tableType) => new IdentityInsertScope(Database, tableType.ParseTable().Table.Name);
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

            public void Dispose() => SetOff();
            private void SetOn() => _database.ExecuteNonQuery($"SET IDENTITY_INSERT {_table.SqlQuote()} ON");
            private void SetOff() => _database.ExecuteNonQuery($"SET IDENTITY_INSERT {_table.SqlQuote()} OFF");
        }
    }
}
