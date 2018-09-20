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
using NPoco;
using NUnit.Framework;


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

        protected virtual void Test<TCreateTables, TAddColumns>()
            => Test(new TableTestCase(typeof(TCreateTables)), 
                (testCase, migrations) => {
                    AddMigrations(testCase, migrations);
                    migrations.AddColumnsMigrationForTableTestCase(new TableTestCase(typeof(TAddColumns)));
                }, TestBetweenUpAndDown);

        protected override void Test(ITableTestCase testCase)
            => Test(testCase, AddMigrations, TestBetweenUpAndDown);

        protected void Test<TCase>(Action<ITableTestCase, IMigrationSet> addMigrations)
            => Test(new TableTestCase<TCase>(), addMigrations, TestBetweenUpAndDown);

        protected void Test<TCase>(Action<ITableTestCase, IMigrationSet> addMigrations, Action<ITableTestCase> testBetweenUpAndDown) 
            => Test(new TableTestCase<TCase>(), addMigrations, testBetweenUpAndDown);

        protected void Test(ITableTestCase testCase, Action<ITableTestCase, IMigrationSet> addMigrations, Action<ITableTestCase> testBetweenUpAndDown)
        {
            var set = Migrator.CreateMigrationSet();
            addMigrations(testCase, set);
            var mig = Migrator.CompileMigrations(set);
            Migrator.Up(mig);
            testBetweenUpAndDown(testCase);
            Migrator.Down(mig);
        }

        protected virtual void AddMigrations(ITableTestCase testCase, IMigrationSet migrations)
            => migrations.AddTableMigrationForTableTestCase(testCase);

        protected virtual void TestBetweenUpAndDown(ITableTestCase testCase)
            => VerifySchemaAgainstModel(testCase);

        protected virtual void VerifySchemaAgainstModel(ITableTestCase testCase)
        {
            foreach (var data in testCase.Datum)
                AssertEx.AreEqual(data.Model, GetTableModelFromDb(data.Model.Name), IsFluentMigrator, IsMigratorDotNet);
        }

        [OneTimeSetUp]
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

        private class IsSparseInfo
        {
            public string Name { get; set; }
            public bool IsSparse { get; set; }
        }

        protected Table GetTableModelFromDb(string tableName)
        {
            var table = new Table {Name = tableName};
            var schema = GetDbSchema();
            var st = schema.Tables.Single(t => t.Name == table.Name);

            var isSparse = new Dictionary<string, bool>(st.Columns.Count, StringComparer.InvariantCultureIgnoreCase);
            using (var db = new Database(ConnectionStringWithDatabase, DatabaseType.SqlServer2012)) {
                var sparseInfo = db.Fetch<IsSparseInfo>("SELECT Name, is_sparse AS IsSparse FROM sys.Columns WHERE object_id=object_id(@0)", tableName);
                foreach (var o in sparseInfo)
                    isSparse.Add(o.Name, o.IsSparse);
            }

            var pkIdx = st.Indexes.SingleOrDefault(i => i.IndexType == "PRIMARY");
            table.PrimaryKeyName = pkIdx.Name;

            foreach (var idx in st.Indexes.Where(i => i.IndexType != "PRIMARY")) {
                var ci = new Index {
                    Name = idx.Name,
                    Unique = idx.IsUnique,
                    Clustered = idx.IndexType != "NONCLUSTERED",
                    Columns = idx.Columns.Select(c => new IndexColumn(c.Name)).ToArray(), 
                };
                table.Indices.Add(ci);
            }

            table.Columns = st.Columns.Select(c => {
                var sqlDbType = (SqlDbType)c.DataType.ProviderDbType;
                var param = new SqlParameter();
                param.SqlDbType = sqlDbType;
                var dbType = param.DbType;

                var fk = st.ForeignKeys.SingleOrDefault(f => f.Columns.Count == 1 && f.Columns[0] == c.Name);

                var defVal = c.DefaultValue;
                if (defVal != null) {
                    if (defVal.StartsWith("(") && defVal.EndsWith(")")) {
                        defVal = defVal.Substring(1);
                        defVal = defVal.Substring(0, defVal.Length - 1);
                    }

                    if (defVal.StartsWith("(") && defVal.EndsWith(")")) {
                        defVal = defVal.Substring(1);
                        defVal = defVal.Substring(0, defVal.Length - 1);
                    }

                    if (dbType != DbType.AnsiString &&
                        dbType != DbType.AnsiStringFixedLength &&
                        dbType != DbType.String &&
                        dbType != DbType.StringFixedLength &&
                        dbType != DbType.Date &&
                        dbType != DbType.DateTime &&
                        dbType != DbType.DateTime2 &&
                        dbType != DbType.DateTimeOffset &&
                        dbType != DbType.Guid &&
                        dbType != DbType.Xml)
                    {
                        if (defVal.StartsWith("'"))
                            defVal = defVal.Substring(1);
                        if (defVal.EndsWith("'"))
                            defVal = defVal.Substring(0, defVal.Length - 1);
                    }
                }
                return new Column {
                    Name = c.Name,
                    Type = dbType,
                    IsPrimaryKey = c.IsPrimaryKey,
                    IsNullable = c.Nullable,
                    IsSparse = isSparse[c.Name],
                    Length = c.Length.IfHasValue(l => l == -1 ? int.MaxValue : l, default(int?)),
                    DefaultValue =
                        dbType == DbType.Boolean
                            ? ((c.DefaultValue?.Contains("0") ?? true) ? "0" : "1")
                            : defVal,
                    AutoIncrement = c.IsAutoNumber
                        ? new AutoIncAttribute(c.IdentityDefinition.IdentitySeed, c.IdentityDefinition.IdentityIncrement)
                        : null,
                    Precision = 
                        (dbType == DbType.Decimal) && (c.Precision.HasValue || c.Scale.HasValue)
                            ? new PrecisionAttribute(c.Precision ?? 0, c.Scale ?? 0) 
                            : ((dbType == DbType.DateTime2 || dbType == DbType.DateTimeOffset) && c.DateTimePrecision != null)
                                ? new PrecisionAttribute(c.DateTimePrecision.Value, 0)
                                : null,
                    ForeignKey = c.IsForeignKey
                        ? new FkAttribute(c.ForeignKeyTableName) {
                                Name = fk.Name,
                                Column = c.ForeignKeyTable.PrimaryKeyColumn.Name,
                            }
                        : null
                };
            }).ToList();
            
            return table;
        }
    }
}
