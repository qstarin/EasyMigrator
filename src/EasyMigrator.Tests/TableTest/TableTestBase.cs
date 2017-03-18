using EasyMigrator.Parsing;
using EasyMigrator.Tests.Schemas;
using NUnit.Framework;


namespace EasyMigrator.Tests.TableTest
{
    abstract public class TableTestBase
    {
        [Test] public void Table1() => Test<Table1>();
        [Test] public void Fk1() => Test<Fk1>();
        [Test] public void ManyToMany1() => Test<ManyToMany1>();
        [Test] public void CustomAutoIncrement_Int32() => Test<CustomAutoIncrement_Int32>();

        protected void Test<TCase>() => Test(new TableTestCase<TCase>());
        abstract protected void Test(ITableTestCase testCase);

        [OneTimeSetUp]
        public virtual void SetupFixture()
        {
            var tableNameFn = Parser.Default.Conventions.TableName;
            Parser.Default.Conventions.TableName = c => {
                if (c.ModelType.Name == "Poco" && c.ModelType.IsNested) {
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
