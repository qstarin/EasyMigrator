using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Extensions;
using EasyMigrator.Parsing;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    abstract public class TableTestBase
    {
        protected IEnumerable<Type> GetAllTableDataTypes()
        {
            return
                GetType().Assembly.GetTypes()
                         .Where(t => t.InheritsFrom<TableTestData>() &&
                                     t != typeof(TableTestData) &&
                                     !t.IsGenericTypeDefinition &&
                                     !t.IsAbstract);
        }

        protected void RunTest(Action<ITableTestData> test, IEnumerable<Type> dataTypes)
        {
            foreach (var t in dataTypes)
                test(new TableTestData(t));
        }

        [TestFixtureSetUp]
        public virtual void SetupFixture()
        {
            var tableNameFn = Parser.Default.Conventions.TableName;
            Parser.Default.Conventions.TableName = c => {
                if (c.ModelType.Name == "Poco" && c.ModelType.IsNested && c.ModelType.DeclaringType.InheritsFrom<ITableTestData>()) {
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

        [TestFixtureTearDown]
        public virtual void TearDownFixture() { }
    }
}
