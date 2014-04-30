using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
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
        protected virtual string DatabaseName { get { return GetType().Name + "Db"; } }
        protected string DatabaseFile { get {  return Path.Combine(Environment.CurrentDirectory, DatabaseName + ".mdf"); } }
        protected string LogFile { get { return Path.Combine(Environment.CurrentDirectory, DatabaseName + ".ldf"); } }
        protected string ConnectionString { get { return ConfigurationManager.ConnectionStrings["Test-LocalDb"].ConnectionString; } }
        protected string ConnectionStringWithDatabase { get { return ConnectionString + string.Format("AttachDbFileName={0};", DatabaseFile); } }
        protected IMigrator Migrator { get { return _getMigrator(ConnectionStringWithDatabase); } }
        private readonly Func<string, IMigrator> _getMigrator;

        protected IntegrationTestBase(Func<string, IMigrator> getMigrator) { _getMigrator = getMigrator; }

        protected DbConnection OpenConnection() { return OpenConnection(ConnectionString); }
        protected DbConnection OpenDatabaseConnection() { return OpenConnection(ConnectionStringWithDatabase); }
        protected DbConnection OpenConnection(string connectionString)
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        override public void SetupFixture()
        {
            base.SetupFixture();

            using (var conn = OpenConnection()) {
                conn.ExecuteNonQuery("USE master;");

                if (File.Exists(DatabaseFile)) {
                    conn.ExecuteNonQuery(string.Format("IF EXISTS(select * from sys.databases where name='{0}') DROP DATABASE [{0}]", DatabaseName));
                    if (File.Exists(DatabaseFile))
                        File.Delete(DatabaseFile);
                }
                if (File.Exists(LogFile)) File.Delete(LogFile);

                conn.ExecuteNonQuery(string.Format(
@"CREATE DATABASE {0} 
ON PRIMARY (NAME={0}_data,FILENAME='{1}') 
LOG ON (NAME={0}_log,FILENAME='{2}');"
                    , DatabaseName, DatabaseFile, LogFile));
            }
        }

        //override public void TearDownFixture() {}

        protected Table GetTableModelFromDb(ITableTestData data)
        {
            using (var conn = OpenConnection()) {
                var columns = conn.GetSchema("Columns", new[] {null, null, data.Model.Name});
            }
            return data.Model;
        }
    }
}
