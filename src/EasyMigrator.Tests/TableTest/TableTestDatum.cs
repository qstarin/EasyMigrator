using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Tests.TableTest
{
    public interface ITableTestDatum : IEnumerable<ITableTestData>
    {
        ITableTestData this[string tableName] { get; }
        ITableTestData this[Type pocoOrTableDataType] { get; }
        void ConditionallyAdd(Type tableDataType);
    }

    public class TableTestDatumList : List<ITableTestData>, ITableTestDatum {
        public ITableTestData this[string tableName] => this.SingleOrDefault(d => d.Model.Name == tableName);

        public ITableTestData this[Type pocoOrTableDataType]
        {
            get {
                var typeAsData = new TableTestData(pocoOrTableDataType);
                return this.SingleOrDefault(d => d.Poco == pocoOrTableDataType || d.Poco == typeAsData.Poco);
            }
        }

        public void ConditionallyAdd(Type tableDataType)
        {
            if (tableDataType == typeof(TableTestCase) || tableDataType == typeof(TableTestData))
                return;
            var d = new TableTestData(tableDataType);
            if (d.Poco != null) // TODO: ? Check for dup table names
                Add(d);
        }
    }
}
