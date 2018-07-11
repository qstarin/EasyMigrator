using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EasyMigrator
{
    public class IndexColumn : Parsing.Model.IIndexColumn
    {
        public IndexColumn(string columnName) : this(columnName, SortOrder.Unspecified) { }
        public IndexColumn(string columnName, SortOrder direction)
        {
            ColumnName = columnName;
            Direction = direction;
        }

        public string ColumnName { get; }
        public string ColumnNameWithDirection => GetColumnNameWithDirection(ColumnName, Direction);
        public SortOrder Direction { get; }

        static private string GetColumnNameWithDirection(string columnName, SortOrder direction)
             => direction == SortOrder.Ascending ? $"{columnName} ASC" : direction == SortOrder.Descending ? $"{columnName} DESC" : columnName;
    }

    public class Ascending : IndexColumn
    {
        public Ascending(string columnName) : base(columnName, SortOrder.Ascending) { }
    }

    public class Descending : IndexColumn
    {
        public Descending(string columnName) : base(columnName, SortOrder.Descending) { }
    }

    public class IndexColumn<TTable>
    {
        public IndexColumn(Expression<Func<TTable, object>> columnExpression) : this(columnExpression, SortOrder.Unspecified) { }
        public IndexColumn(Expression<Func<TTable, object>> columnExpression, SortOrder direction)
        {
            ColumnExpression = columnExpression;
            Direction = direction;
        }

        public Expression<Func<TTable, object>> ColumnExpression { get; }
        public SortOrder Direction { get; }
    }

    public class Ascending<TTable> : IndexColumn<TTable>
    {
        public Ascending(Expression<Func<TTable, object>> columnExpression) : base(columnExpression, SortOrder.Ascending) { }
    }

    public class Descending<TTable> : IndexColumn<TTable>
    {
        public Descending(Expression<Func<TTable, object>> columnExpression) : base(columnExpression, SortOrder.Descending) { }
    }
}
