using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyMigrator.Model;


namespace EasyMigrator.Extensions
{
    static public class ModelExtensions
    {
        static public IEnumerable<Column> DefinedInPoco(this IEnumerable<Column> columns) { return columns.Where(c => c.DefinedInPoco); }
    }
}
