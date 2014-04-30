using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace EasyMigrator.Tests.Integration
{
    public interface IMigrator
    {
        void Up(Type poco);
    }
}
