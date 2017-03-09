using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Tests.TableTest
{
    public class MigrationOrderAttribute : Attribute
    {
        public int Order { get; private set; }
        public MigrationOrderAttribute(int order) { Order = order; }
    }
}
