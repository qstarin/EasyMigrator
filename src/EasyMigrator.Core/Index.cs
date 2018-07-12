using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EasyMigrator
{
    public class Index : Parsing.Model.IIndex
    {
        public string Name { get; set; }
        public bool Clustered { get; set; }
        public bool Unique { get; set; }
        public string Where { get; set; }
        public string With { get; set; }
        public IndexColumn[] Columns { get; set; }
        public IndexColumn[] Includes { get; set; }
        Parsing.Model.IIndexColumn[] Parsing.Model.IIndex.Columns => Columns;
        Parsing.Model.IIndexColumn[] Parsing.Model.IIndex.Includes => Includes;

        public Index() { }
        public Index(params string[] columnNamesAndDirection) 
            : this(ConvertToColumns(columnNamesAndDirection).ToArray(), null) { }
        public Index(string[] columnNamesAndDirection, string[] includes) 
            : this(ConvertToColumns(columnNamesAndDirection).ToArray(), ConvertToColumns(includes).ToArray()) { }
        public Index(params IndexColumn[] columns) 
            : this(columns, null) { }
        public Index(IndexColumn[] columns, IndexColumn[] includes)
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

    public class Index<TTable>
    {
        public string Name { get; set; }
        public bool Clustered { get; set; }
        public bool Unique { get; set; }
        public string Where { get; set; }
        public string With { get; set; }
        public IndexColumn<TTable>[] Columns { get; set; }
        public IndexColumn<TTable>[] Includes { get; set; }

        public Index() { }
        public Index(params Expression<Func<TTable, object>>[] columns) 
            : this(columns.Select(c => new IndexColumn<TTable>(c)).ToArray()) { }
        public Index(Expression<Func<TTable, object>>[] columns, Expression<Func<TTable, object>>[] includes) 
            : this(columns.Select(c => new IndexColumn<TTable>(c)).ToArray(), includes.Select(c => new IndexColumn<TTable>(c)).ToArray()) { }
        public Index(params IndexColumn<TTable>[] columns) 
            : this(columns, null) { }
        public Index(IndexColumn<TTable>[] columns, IndexColumn<TTable>[] includes)
        {
            Columns = columns;
            Includes = includes ?? new IndexColumn<TTable>[0];
        }
    }
}
