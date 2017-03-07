using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public interface IIndex
    {
        string Name { get; set; }
        bool Unique { get; set; }
        bool Clustered { get; set; }
    }
}
