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
        public IndexColumn[] Includes { get; }

        public CompositeIndex(params string[] columnNamesAndDirection) 
            : this(ConvertToColumns(columnNamesAndDirection).ToArray(), null) { }
        public CompositeIndex(string[] columnNamesAndDirection, string[] includes) 
            : this(ConvertToColumns(columnNamesAndDirection).ToArray(), ConvertToColumns(includes).ToArray()) { }
        public CompositeIndex(params IndexColumn[] columns) 
            : this(columns, null) { }
        public CompositeIndex(IndexColumn[] columns, IndexColumn[] includes)
        {
            Columns = columns;
            Includes = includes ?? new IndexColumn[0];
        }

        static protected IEnumerable<IndexColumn> ConvertToColumns(IEnumerable<string> columnNamesWithDirection)
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
        public IndexColumn<TTable>[] Includes { get; }

        public CompositeIndex(params Expression<Func<TTable, object>>[] columns) 
            : this(columns.Select(c => new IndexColumn<TTable>(c)).ToArray()) { }
        public CompositeIndex(Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes) 
            : this(columns.Select(c => new IndexColumn<TTable>(c)).ToArray(), includes.Select(c => new IndexColumn<TTable>(c)).ToArray()) { }
        public CompositeIndex(params IndexColumn<TTable>[] columns) 
            : this(columns, null) { }
        public CompositeIndex(IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes)
        {
            Columns = columns;
            Includes = includes ?? new IndexColumn<TTable>[0];
        }
    }
}
