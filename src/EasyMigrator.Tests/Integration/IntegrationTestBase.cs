using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NUnit.Framework;


namespace EasyMigrator.Tests.Integration
{
    abstract public class IntegrationTestBase
    {
        protected SqlConnection Connection { get; private set; }
        protected string DatabaseName { get; private set; }

        [TestFixtureSetUp]
        public virtual void SetupFixture()
        {
            DatabaseName = this.GetType().Name + "Db";
            var connString = ConfigurationManager.ConnectionStrings["Test-LocalDb"];
            Connection = new SqlConnection(connString.ConnectionString);
            Connection.Open();
            ExecuteNonQuery(string.Format("IF EXISTS(select * from sys.databases where name='{0}') DROP DATABASE [{0}]", DatabaseName));
            ExecuteNonQuery(string.Format("CREATE DATABASE {0}", DatabaseName));
            ExecuteNonQuery(string.Format("USE {0}", DatabaseName));
        }

        [TestFixtureTearDown]
        public virtual void TearDownFixture()
        {
            ExecuteNonQuery("USE master");
            ExecuteNonQuery(string.Format("DROP DATABASE {0}", DatabaseName));
            Connection.Close();
        }

        protected void ExecuteNonQuery(string sql)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }
    }
}
