using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EasyMigrator.Model
{
    public class Table
    {
        public Table()
        {
            PrimaryKey = new List<Column>();
            Columns = new List<Column>();
            ForeignKeys = new List<ForeignKey>();
            Indexes = new List<Index>();
        }

        public string Name { get; set; }
        public ICollection<Column> PrimaryKey { get; set; }
        public ICollection<Column> Columns { get; set; }
        public ICollection<ForeignKey> ForeignKeys { get; set; }
        public ICollection<Index> Indexes { get; set; }
    }

    public class Column
    {
        public string Name { get; set; }
        public DbType Type { get; set; }
        public int Size { get; set; }
        //public ColumnProperty ColumnProperty { get; set; }
        public object DefaultValue { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNullable { get; set; }
    }

    public class ForeignKey
    {
        public string Name { get; set; }
        public string FkTable { get; set; }
        public string FkColumn { get; set; }
        public string PkTable { get; set; }
        public string PkColumn { get; set; }
    }

    public class Index
    {
        public string Name { get; set; }
        public bool Unique { get; set; }
        public bool Clustered { get; set; }
        public string Table { get; set; }
        public string[] Columns { get; set; }
    }
}
