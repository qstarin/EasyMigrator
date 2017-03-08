using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public class Table
    {
        public string Name { get; set; }
        public bool HasPrimaryKey => PrimaryKeyColumns.Any();
        public string PrimaryKeyName { get; set; }
        public IList<Column> Columns { get; set; } = new List<Column>();
        public IEnumerable<Column> PrimaryKeyColumns => Columns.Where(c => c.IsPrimaryKey);
    }
}
