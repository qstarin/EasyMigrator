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
        void Up(IEnumerable<Type> pocos);
        void Up(Action<NPoco.Database> action);
        void Up(IEnumerable<Action<NPoco.Database>> actions);
        void Down(Type poco);
        void Down(IEnumerable<Type> pocos);
        void Down(Action<NPoco.Database> action);
        void Down(IEnumerable<Action<NPoco.Database>> actions);

        IMigrationSet CreateMigrationSet();
        ICompiledMigrations CompileMigrations(IMigrationSet migrations);
        void Up(ICompiledMigrations migrations);
        void Down(ICompiledMigrations migrations);
    }
}
