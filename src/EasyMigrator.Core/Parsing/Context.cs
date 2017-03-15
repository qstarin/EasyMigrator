using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Parsing
{
    public class Context
    {
        public Conventions Conventions { get; set; }
        public Table Table { get; set; }
        public Type ModelType { get; set; }
        public IEnumerable<FieldInfo> Fields { get; set; }
        public IDictionary<FieldInfo, Column> Columns { get; set; } = new Dictionary<FieldInfo, Column>();
        public object Model { get; set; }
    }
}
