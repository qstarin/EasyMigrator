using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyMigrator.Extensions;


namespace EasyMigrator.Parsing.Model
{
    public class Table
    {
        public string Name { get; set; }
        public bool HasPrimaryKey => Columns.PrimaryKey().Any();
        public string PrimaryKeyName { get; set; }
        public bool PrimaryKeyIsClustered { get; set; } = true;
        public IList<Column> Columns { get; set; } = new List<Column>();
        public IList<IIndex> Indices { get; set; } = new List<IIndex>();
    }
}
