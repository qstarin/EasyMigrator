using System.Reflection;


namespace EasyMigrator.Tests.Integration.MigratorDotNet
{
    public class CompiledMigrations : ICompiledMigrations
    {
        public Assembly MigrationsAssembly { get; }
        public CompiledMigrations(Assembly migrationsAssembly) { MigrationsAssembly = migrationsAssembly; }
    }
}