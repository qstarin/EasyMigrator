using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace EasyMigrator.Tests.Integration
{
    public interface ICompiledMigrations { }
}


namespace EasyMigrator.Tests.Integration.FluentMigrator
{

    public class CompiledMigrations : ICompiledMigrations
    {
        public IList<global::FluentMigrator.Migration> Migrations { get; }
        public CompiledMigrations(IList<global::FluentMigrator.Migration> migrations) { Migrations = migrations; }
    }
}


namespace EasyMigrator.Tests.Integration.MigratorDotNet
{
    public class CompiledMigrations : ICompiledMigrations
    {
        public Assembly MigrationsAssembly { get; }
        public CompiledMigrations(Assembly migrationsAssembly) { MigrationsAssembly = migrationsAssembly; }
    }
}