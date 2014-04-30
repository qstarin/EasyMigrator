using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace EasyMigrator.Tests
{
    internal static class Extensions
    {
        static public void ExecuteNonQuery(this DbConnection connection, string sql)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }
        static public T ExecuteScalar<T>(this DbConnection connection, string sql)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return (T)cmd.ExecuteScalar();
        }
    }
}
