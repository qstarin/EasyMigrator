using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public interface IAutoIncrement
    {
        long Seed { get; }
        long Step { get; }
    }
}
