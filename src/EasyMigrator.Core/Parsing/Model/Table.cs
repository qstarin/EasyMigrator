using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public class Table
    {
        public string Name { get; set; }
        public ICollection<Column> Columns { get; set; } = new List<Column>();
    }
}
