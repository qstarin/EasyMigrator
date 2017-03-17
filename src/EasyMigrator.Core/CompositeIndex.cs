using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EasyMigrator
{
    public class CompositeIndex
    {
        public IndexColumn[] Columns { get; }
        public CompositeIndex(params string[] columnNamesAndDirection) : this(ConvertToColumns(columnNamesAndDirection).ToArray()) { }
        public CompositeIndex(params IndexColumn[] columns) { Columns = columns; }

        static private IEnumerable<IndexColumn> ConvertToColumns(IEnumerable<string> columnNamesWithDirection)
        {
            foreach (var c in columnNamesWithDirection) {
                if (c.EndsWith(" ASC"))
                    yield return new IndexColumn(c.Substring(0, c.Length - " ASC".Length), SortOrder.Ascending);
                else if (c.EndsWith(" DESC"))
                    yield return new IndexColumn(c.Substring(0, c.Length - " DESC".Length), SortOrder.Descending);
                else
                    yield return new IndexColumn(c);
            }
        }
    }

    public class CompositeIndex<TTable>
    {
        public IndexColumn<TTable>[] Columns { get; }
        public CompositeIndex(params Expression<Func<TTable, object>>[] columns) : this(columns.Select(c => new IndexColumn<TTable>(c)).ToArray()) { }
        public CompositeIndex(params IndexColumn<TTable>[] columns) { Columns = columns; }
    }
}
