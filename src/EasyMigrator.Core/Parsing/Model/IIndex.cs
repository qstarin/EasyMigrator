using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public interface IIndex
    {
        string Name { get; }
        bool Clustered { get; }
        bool Unique { get; }
        string Where { get; }
        string With { get; }
        IIndexColumn[] Columns { get; }
        IIndexColumn[] Includes { get; }
    }
}
