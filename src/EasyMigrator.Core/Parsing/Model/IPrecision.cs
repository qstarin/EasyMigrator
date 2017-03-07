using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public interface IPrecision
    {
        int Precision { get; }
        int Scale { get; }
    }
}
