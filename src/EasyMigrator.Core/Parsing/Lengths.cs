using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing
{
    public class Lengths
    {
        public int Default { get; set; }
        public int Short { get; set; }
        public int Medium { get; set; }
        public int Long { get; set; }
        public int Max { get; set; }

        public int this[Length length]
        {
            get {
                switch (length) {
                    case Length.Short: return Short;
                    case Length.Medium: return Medium;
                    case Length.Long: return Long;
                    case Length.Max: return Max;
                    default: return Default;
                }
            }
        }
    }
}
