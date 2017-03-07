using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
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
        internal bool DefinedInPoco { get; set; }
    }
}
