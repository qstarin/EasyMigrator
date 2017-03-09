using EasyMigrator.Parsing;
using NUnit.Framework;


namespace EasyMigrator.Tests.TableTest
{
    abstract public class TableTestBase
    {
        protected void Test<TCase>() { Test(new TableTestCase<TCase>()); }
        abstract protected void Test(ITableTestCase testCase);

        [OneTimeSetUp]
        public virtual void SetupFixture()
        {
            var tableNameFn = Parser.Default.Conventions.TableName;
            Parser.Default.Conventions.TableName = c => {
                if (c.ModelType.Name == "Poco" && c.ModelType.IsNested /*&& c.ModelType.DeclaringType.InheritsFrom<ITableTestData>()*/) {
                    var modelType = c.ModelType;
                    c.ModelType = modelType.DeclaringType;
                    var tableName = tableNameFn(c);
                    c.ModelType = modelType;
                    return tableName;
                }
                else
                    return tableNameFn(c);
            };
        }

        [OneTimeTearDown]
        public virtual void TearDownFixture() { }
    }
}
