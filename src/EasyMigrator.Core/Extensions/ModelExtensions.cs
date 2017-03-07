using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Parsing.Model;


namespace EasyMigrator.Extensions
{
    static public class ModelExtensions
    {
        static public IEnumerable<Column> DefinedInPoco(this IEnumerable<Column> columns) => columns.Where(c => c.DefinedInPoco);
    }
}
