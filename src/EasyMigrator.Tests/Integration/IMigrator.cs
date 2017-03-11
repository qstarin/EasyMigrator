using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace EasyMigrator.Tests.Integration
{
    public interface IMigrator
    {
        IMigrationSet CreateMigrationSet();
        ICompiledMigrations CompileMigrations(IMigrationSet migrations);
        void Up(ICompiledMigrations migrations);
        void Down(ICompiledMigrations migrations);
    }
}
