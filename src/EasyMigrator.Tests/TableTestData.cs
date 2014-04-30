using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyMigrator.Model;


namespace EasyMigrator.Tests
{
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
                        .GetValue(type == GetType() ? this : Activator.CreateInstance(type)) as Table;
            Poco = type.GetNestedType("Poco", BindingFlags.NonPublic | BindingFlags.Public);
        }
    }

    public class TableTestData<T> : TableTestData 
    {
        public TableTestData() : base(typeof(T)) { }
    }
}
