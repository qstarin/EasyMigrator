using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyMigrator.Extensions;
using EasyMigrator.Parsing.Model;
using FluentMigrator;


namespace EasyMigrator.Tests
{
    public class MigrationOrderAttribute : Attribute
    {
        public int Order { get; private set; }
        public MigrationOrderAttribute(int order) { Order = order; }
    }

    public interface ITableTestCase
    {
        ITableTestDatum Datum { get; }
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
                datum.ConditionallyAdd(GetType());
        }

        private static IEnumerable<ITableTestCase> All
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

    public class TableTestCase<T> : TableTestCase
    {
        public TableTestCase() : base(typeof(T)) { }
    }

    public interface ITableTestDatum : IEnumerable<ITableTestData>
    {
        ITableTestData this[string tableName] { get; }
        ITableTestData this[Type pocoOrTableDataType] { get; }
        void ConditionallyAdd(Type tableDataType);
    }

    public class TableTestDatumList : List<ITableTestData>, ITableTestDatum {
        public ITableTestData this[string tableName]
        {
            get { return this.SingleOrDefault(d => d.Model.Name == tableName); }
        }

        public ITableTestData this[Type pocoOrTableDataType]
        {
            get {
                var typeAsData = new TableTestData(pocoOrTableDataType);
                return this.SingleOrDefault(d =>
                                            d.Poco == pocoOrTableDataType ||
                                            d.Poco == typeAsData.Poco);
            }
        }

        public void ConditionallyAdd(Type tableDataType)
        {
            if (tableDataType == typeof(TableTestCase) || tableDataType == typeof(TableTestData))
                return;
            var d = new TableTestData(tableDataType);
            if (d.Model != null && d.Poco != null) // TODO: ? Check for dup table names
                Add(d);
        }
    }


    public interface ITableTestData
    {
        Table Model { get; }
        Type Poco { get; }
    }

    public class TableTestData : ITableTestData
    {
        public Table Model { get; private set; }
        public Type Poco { get; private set; }

        protected TableTestData() { Initialize(GetType()); }
        public TableTestData(Type type) { Initialize(type); }

        private void Initialize(Type type)
        {
            Model = type.GetField("Model",
                                  BindingFlags.NonPublic | BindingFlags.Public |
                                  BindingFlags.Instance | BindingFlags.Static)
                        .IfNotNull(t => t.GetValue(type == GetType()
                                                       ? this
                                                       : Activator.CreateInstance(type)) as Table);
            Poco = type.GetNestedType("Poco", BindingFlags.NonPublic | BindingFlags.Public);
        }
    }

    public class TableTestData<T> : TableTestData 
    {
        public TableTestData() : base(typeof(T)) { }
    }
}
