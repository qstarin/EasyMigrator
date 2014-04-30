using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace EasyMigrator.Tests.Integration
{
    public interface IMigrator
    {
        void Up(Action<NPoco.Database> action);
        void Down(Action<NPoco.Database> action);
        void Up(Type poco);
        void Down(Type poco);
    }
}
