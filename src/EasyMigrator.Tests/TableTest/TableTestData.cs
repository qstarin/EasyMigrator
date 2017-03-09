using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyMigrator.Extensions;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Tests.TableTest
{
    public interface ITableTestData
    {
        Table Model { get; }
        Type Poco { get; }
    }

    public class TableTestData<T> : TableTestData
    {
        public TableTestData() : base(typeof(T)) { }
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
}
