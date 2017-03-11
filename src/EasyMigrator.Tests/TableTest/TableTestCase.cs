using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyMigrator.Extensions;


namespace EasyMigrator.Tests.TableTest
{
    public interface ITableTestCase
    {
        ITableTestDatum Datum { get; }
    }

    public class TableTestCase<T> : TableTestCase
    {
        public TableTestCase() : base(typeof(T)) { }
    }

    public class TableTestCase : ITableTestCase
    {
        public ITableTestDatum Datum { get; private set; }

        protected TableTestCase() { Initialize(GetType()); }
        public TableTestCase(Type type) { Initialize(type); }

        private void Initialize(Type type)
        {
            var datum = new TableTestDatumList();
            Datum = datum;
            var typesToScan =
                type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)
                    .OrderBy(t => t.GetAttribute<MigrationOrderAttribute>()
                                   .IfNotNull(a => a.Order, int.MaxValue));

            foreach (var t in typesToScan)
                datum.ConditionallyAdd(t);

            if (datum.Count == 0)
                datum.ConditionallyAdd(type);
        }

        static private IEnumerable<ITableTestCase> All
        {
            get {
                return typeof(TableTestCase)
                    .Assembly.GetTypes()
                    .Where(t => t.InheritsFrom<ITableTestCase>() &&
                                t != typeof (TableTestCase) &&
                                !t.IsGenericTypeDefinition &&
                                !t.IsAbstract)
                    .Select(t => new TableTestCase(t));
            }
        }
    }
}
