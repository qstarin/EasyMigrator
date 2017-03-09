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
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Tests.Integration
{
    public enum MigrationDirection { Up, Down }

    abstract public class IntegrationTestBase : TableTestBase
    {
        protected virtual string DatabaseName => GetType().Name + "Db";
        protected string DatabaseFile => Path.Combine(Environment.CurrentDirectory, DatabaseName + ".mdf");
        protected string LogFile => Path.Combine(Environment.CurrentDirectory, DatabaseName + ".ldf");
        protected ConnectionStringSettings ConnectionStringSettings => ConfigurationManager.ConnectionStrings["Test-LocalDb"];
        protected string ConnectionString => ConnectionStringSettings.ConnectionString;
        protected string ConnectionStringWithDatabase => ConnectionString + $"AttachDbFileName={DatabaseFile};";
        protected DbConnection OpenConnection() => OpenConnection(ConnectionString);
        protected DbConnection OpenDatabaseConnection() => OpenConnection(ConnectionStringWithDatabase);

        protected DbConnection OpenConnection(string connectionString)
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        protected IMigrator Migrator => _getMigrator(ConnectionStringWithDatabase);
        private readonly Func<string, IMigrator> _getMigrator;

        protected IntegrationTestBase(Func<string, IMigrator> getMigrator) { _getMigrator = getMigrator; }


        public override void SetupFixture()
        {
            base.SetupFixture();

            using (var conn = OpenConnection()) {
                conn.ExecuteNonQuery("USE master;");

                if (File.Exists(DatabaseFile)) {
                    DeleteDatabase(conn);
                    if (File.Exists(DatabaseFile))
                        File.Delete(DatabaseFile);
                }

                if (File.Exists(LogFile))
                    File.Delete(LogFile);

                try {
                    CreateDatabase(conn);
                }
                catch (SqlException ex) {
                    if (ex.Message.StartsWith($"Database '{DatabaseName}' already exists")) {
                        DeleteDatabase(conn);
                        CreateDatabase(conn);
                    }
                }
            }
        }

        private void CreateDatabase(DbConnection conn)
            => conn.ExecuteNonQuery(
                $"CREATE DATABASE {DatabaseName} ON PRIMARY " +
                $"(NAME={DatabaseName}_data,FILENAME='{DatabaseFile}') " + 
                $"LOG ON (NAME={DatabaseName}_log,FILENAME='{LogFile}');");

        private void DeleteDatabase(DbConnection conn)
            => conn.ExecuteNonQuery($"IF EXISTS(select * from sys.databases where name='{DatabaseName}') DROP DATABASE [{DatabaseName}]");

        protected Table GetTableModelFromDb(string tableName)
        {
            var table = new Table {Name = tableName};
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
