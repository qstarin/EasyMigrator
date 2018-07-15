using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public interface IForeignKey
    {
        string Name { get; set; }
        string Table { get; }
        string Column { get; set; }
        Rule? OnDelete { get; set; }
        Rule? OnUpdate { get; set; }
    }
}
