using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public class Index
    {
        public string Name { get; set; }
        public bool Clustered { get; set; }
        public bool Unique { get; set; }
        public IIndexColumn[] Columns { get; set; }
        public IIndexColumn[] Includes { get; set; }
    }
}
