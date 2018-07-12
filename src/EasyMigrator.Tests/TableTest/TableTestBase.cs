using System;
using EasyMigrator.Parsing;
using EasyMigrator.Tests.Schemas;
using NUnit.Framework;


namespace EasyMigrator.Tests.TableTest
{
    abstract public class TableTestBase
    {
        protected void Test<TCase>() => Test(new TableTestCase<TCase>());
        abstract protected void Test(ITableTestCase testCase);

        private IDisposable _conventionsScope;

        [OneTimeSetUp]
        public virtual void SetupFixture()
        {
            var tableNameFn = Parser.Current.Conventions.TableName;
            _conventionsScope = Parser.Override(conv => 
                conv.TableName = c => {
                    if (c.ModelType.Name == "Poco" && c.ModelType.IsNested) {
                        var modelType = c.ModelType;
                        c.ModelType = modelType.DeclaringType;
                        var tableName = tableNameFn(c);
                        c.ModelType = modelType;
                        return tableName;
                    }
                    else
                        return tableNameFn(c);
                });
        }

        [OneTimeTearDown]
        public virtual void TearDownFixture() { _conventionsScope?.Dispose(); }
    }
}
