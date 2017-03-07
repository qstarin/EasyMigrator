using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Parsing
{
    public partial class Conventions
    {
        public Func<Context, string> TableName { get; set; }
        public Func<Context, Column> PrimaryKey { get; set; }
        public ITypeMap TypeMap { get; set; }
        public Lengths StringLengths { get; set; }
        public Lengths PrecisionLengths { get; set; }
        public Func<Context, IPrecision> DefaultPrecision { get; set; }
        public bool IndexForeignKeys { get; set; }
    }
}
