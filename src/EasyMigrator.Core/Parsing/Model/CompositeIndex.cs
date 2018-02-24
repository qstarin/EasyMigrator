using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public class CompositeIndex
    {
        public string Name { get; set; }
        public IndexColumn[] Columns { get; set; }
        public IndexColumn[] Includes { get; set; }
        public bool Unique { get; set; }
        public bool Clustered { get; set; }
    }
}
