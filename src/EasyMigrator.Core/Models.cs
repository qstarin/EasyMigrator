using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace EasyMigrator.Model
{
    public class Table
    {
        public Table() { Columns = new List<Column>(); }

        public string Name { get; set; }
        public ICollection<Column> Columns { get; set; }
    }

    public class Column
    {
        public string Name { get; set; }
        public DbType Type { get; set; }
        public object DefaultValue { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public IAutoIncrement AutoIncrement { get; set; }
        public int? Length { get; set; }
        public IPrecision Precision { get; set; }
        public IIndex Index { get; set; }
        public IForeignKey ForeignKey { get; set; }
    }

    public interface IAutoIncrement
    {
        long? Seed { get; set; }
        long? Step { get; set; }
    }

    public interface IPrecision
    {
        byte? Scale { get; set; }
        byte? Precision { get; set; }
    }

    public interface IForeignKey
    {
        string Name { get; set; }
        string Table { get; }
        string Column { get; set; }
    }

    public interface IIndex
    {
        string Name { get; set; }
        bool Unique { get; set; }
        bool Clustered { get; set; }
    }
}
