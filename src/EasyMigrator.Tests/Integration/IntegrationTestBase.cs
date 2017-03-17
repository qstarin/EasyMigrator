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
using EasyMigrator.Tests.TableTest;


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

        protected bool IsMigratorDotNet => Migrator is MigratorDotNet.Migrator;
        protected bool IsFluentMigrator => Migrator is FluentMigrator.Migrator;

        protected override void Test(ITableTestCase testCase)
        {
            var set = Migrator.CreateMigrationSet();
            set.AddMigrationForTableTestCase(testCase);
            var mig = Migrator.CompileMigrations(set);
            Migrator.Up(mig);
            TestBetweenUpAndDown(testCase);
            Migrator.Down(mig);
        }

        protected virtual void TestBetweenUpAndDown(ITableTestCase testCase)
        {
            foreach (var data in testCase.Datum)
                AssertEx.AreEqual(data.Model, GetTableModelFromDb(data.Model.Name), IsMigratorDotNet);
        }

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

        protected DatabaseSchema GetDbSchema() => new DatabaseReader(ConnectionStringWithDatabase, SqlType.SqlServer).ReadAll();

        protected Table GetTableModelFromDb(string tableName)
        {
            var table = new Table {Name = tableName};
            var schema = GetDbSchema();
            var st = schema.Tables.Single(t => t.Name == table.Name);

            var pkIdx = st.Indexes.SingleOrDefault(i => i.IndexType == "PRIMARY");
            table.PrimaryKeyName = pkIdx.Name;

            foreach (var idx in st.Indexes.Where(i => i.IndexType != "PRIMARY" && i.Columns.Count > 1)) {
                var ci = new Parsing.Model.CompositeIndex {
                    Name = idx.Name,
                    Unique = idx.IsUnique,
                    Clustered = idx.IndexType != "NONCLUSTERED",
                    Columns = idx.Columns.Select(c => new IndexColumn(c.Name)).ToArray()
                };
                table.CompositeIndices.Add(ci);
            }

            table.Columns = st.Columns.Select(c => {
                var sqlDbType = (SqlDbType)c.DataType.ProviderDbType;
                var param = new SqlParameter();
                param.SqlDbType = sqlDbType;
                var dbType = param.DbType;

                var idx = st.Indexes.SingleOrDefault(i => i.IndexType != "PRIMARY" && i.Columns.Count == 1 && i.Columns[0].Name == c.Name);
                var fk = st.ForeignKeys.SingleOrDefault(f => f.Columns.Count == 1 && f.Columns[0] == c.Name);

                return new Column {
                    Name = c.Name,
                    Type = dbType,
                    IsPrimaryKey = c.IsPrimaryKey,
                    IsNullable = c.Nullable,
                    Length = c.Length.IfHasValue(l => l == -1 ? int.MaxValue : l, (int?)null),
                    DefaultValue =
                        dbType == DbType.Boolean
                            ? ((c.DefaultValue?.Contains("0") ?? true) ? "0" : "1")
                            : (object)c.DefaultValue,
                    AutoIncrement = c.IsAutoNumber
                        ? new AutoIncAttribute(c.IdentityDefinition.IdentitySeed, c.IdentityDefinition.IdentityIncrement)
                        : null,
                    Precision = dbType == DbType.Decimal && (c.Precision.HasValue || c.Scale.HasValue)
                        ? new PrecisionAttribute(c.Precision.Value, c.Scale.Value) 
                        : null,
                    Index = idx != null
                        ? idx.IndexType != "NONCLUSTERED"
                            ? new ClusteredAttribute { Name = idx.Name }
                            : c.IsUniqueKey 
                                ? new UniqueAttribute { Name = idx.Name } 
                                : new IndexAttribute { Name = idx.Name }
                        : null,
                    ForeignKey = c.IsForeignKey
                        ? new FkAttribute(c.ForeignKeyTableName) {
                                Name = fk.Name,
                                Column = c.ForeignKeyTable.PrimaryKeyColumn.Name
                            }
                        : null
                };
            }).ToList();
            
            return table;
        }
    }
}
