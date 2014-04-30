using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using EasyMigrator.Extensions;
using EasyMigrator.Model;
using EasyMigrator.Parsing;
using EasyMigrator.Tests.Data;
using NUnit.Framework;


namespace EasyMigrator.Tests.Integration
{
    abstract public class IntegrationTestBase : TableTestBase
    {
        abstract protected IMigrator Migrator { get; }
        protected virtual string DatabaseName { get { return GetType().Name + "Db"; } }
        static public string ConnectionString { get { return ConfigurationManager.ConnectionStrings["Test-LocalDb"].ConnectionString; } }
        protected IDbConnection GetConnection() { return new SqlConnection(ConnectionString); }

        override public void SetupFixture()
        {
            base.SetupFixture();

            using (var conn = GetConnection()) { 
                conn.Open();
                conn.ExecuteNonQuery(string.Format("IF EXISTS(select * from sys.databases where name='{0}') DROP DATABASE [{0}]", DatabaseName));
                conn.ExecuteNonQuery(string.Format("CREATE DATABASE {0}", DatabaseName));
                conn.ExecuteNonQuery(string.Format("USE {0}", DatabaseName));
            }
        }

        override public void TearDownFixture()
        {
            using (var conn = GetConnection()) {
                conn.Open();
                conn.ExecuteNonQuery("USE master");
                conn.ExecuteNonQuery(string.Format("DROP DATABASE {0}", DatabaseName));
            }
        }

        protected Table GetTableModelFromDb(ITableTestData data)
        {
            return data.Model;
        }
    }



    internal static class ConnectionExtensions
    {
        static public void ExecuteNonQuery(this IDbConnection connection, string sql)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }
    }
}
