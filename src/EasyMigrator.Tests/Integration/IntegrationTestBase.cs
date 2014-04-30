using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
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
        protected ConnectionStringSettings ConnectionStringSettings { get {  return ConfigurationManager.ConnectionStrings["Test-LocalDb"]; } }
        protected string ConnectionString { get { return ConnectionStringSettings.ConnectionString; } }
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

        protected Table GetTableModelFromDb(ITableTestData data)
        {
            var table = new Table {Name = data.Model.Name};
            var schema = new DatabaseReader(ConnectionStringWithDatabase, SqlType.SqlServer).ReadAll();
            var st = schema.Tables.Single(t => t.Name == table.Name);
            table.Columns = st.Columns.Select(c => {
                var sqlDbType = (SqlDbType)c.DataType.ProviderDbType;
                var param = new SqlParameter();
                param.SqlDbType = sqlDbType;
                var dbType = param.DbType;

                return new Column {
                    Name = c.Name,
                    Type = dbType,//(DbType)Enum.Parse(typeof(DbType), c.DbDataType),
                    IsPrimaryKey = c.IsPrimaryKey,
                    IsNullable = c.Nullable,
                    Length = c.Length.IfHasValue(l => l == -1 ? int.MaxValue : l, (int?)null),
                    DefaultValue = c.DefaultValue,
                    AutoIncrement = c.IsAutoNumber
                        ? new AutoIncAttribute(c.IdentityDefinition.IdentitySeed, c.IdentityDefinition.IdentityIncrement)
                        : null,
                    Precision = dbType == DbType.Decimal && (c.Precision.HasValue || c.Scale.HasValue)
                        ? new PrecisionAttribute(c.Precision.Value, c.Scale.Value) 
                        : null,
                    Index = c.IsIndexed && !c.IsPrimaryKey
                        ? new IndexAttribute { Unique = c.IsUniqueKey }
                        : null,
                    ForeignKey = c.IsForeignKey
                        ? new FkAttribute(c.ForeignKeyTableName) { Column = c.ForeignKeyTable.PrimaryKeyColumn.Name }
                        : null
                };
            }).ToList();
            
            return table;
        }
    }
}
