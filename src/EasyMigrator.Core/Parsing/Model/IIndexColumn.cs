using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace EasyMigrator.Parsing.Model
{
    public interface IIndexColumn
    {
        string ColumnName { get; }
        string ColumnNameWithDirection { get; }
        SortOrder Direction { get; }
    }
}
