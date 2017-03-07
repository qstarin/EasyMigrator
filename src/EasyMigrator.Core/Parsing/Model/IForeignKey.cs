using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    // TODO: Add cascade options
    public interface IForeignKey
    {
        string Name { get; set; }
        string Table { get; }
        string Column { get; set; }
    }
}
