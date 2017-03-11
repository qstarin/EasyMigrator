using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Migrator.Framework;


namespace EasyMigrator.Tests.Integration.MigratorDotNet
{
    public class Migrator : IMigrator
    {
        private readonly string _connectionString;
        public Migrator(string connectionString) { _connectionString = connectionString; }


        public IMigrationSet CreateMigrationSet() => new MigrationSet();

        public ICompiledMigrations CompileMigrations(IMigrationSet migrations)
        {
            var set = migrations as MigrationSet;
            if (set == null)
                throw new InvalidOperationException("Migration set is not a MigratorDotNet migration set.");

            return new CompiledMigrations(BuildMigrationAssembly(set.MigrationActions.ToList()));
        }

        public void Up(ICompiledMigrations migrations)
            => BuildRunner(migrations).MigrateToLastVersion();

        public void Down(ICompiledMigrations migrations)
            => BuildRunner(migrations).MigrateTo(0); // verify this is ok

        private global::Migrator.Migrator BuildRunner(ICompiledMigrations migrations)
        {
            var mig = migrations as CompiledMigrations;
            if (mig == null)
                throw new InvalidOperationException("Compiled migrations are not for MigratorDotNet.");

            return new global::Migrator.Migrator("SqlServer", _connectionString, mig.MigrationsAssembly);
        }

        static private readonly Dictionary<string, IList<MigrationActions>> _migrationActions = new Dictionary<string, IList<MigrationActions>>();
        static public MigrationActions GetMigrationAction(string assemblyName, int version)
        {
            if (!_migrationActions.ContainsKey(assemblyName))
                return null;

            var migrations = _migrationActions[assemblyName];
            return migrations[version - 1];
        }

        private Assembly BuildMigrationAssembly(IList<MigrationActions> migrationActionsList)
        {
            var assemblyName = $"mdn_test_{Guid.NewGuid()}";
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name, assemblyName + ".dll");

            for (int i = 0; i < migrationActionsList.Count; i++)
                BuildMigrationClass(assemblyName, moduleBuilder, i + 1);

            assemblyBuilder.Save($@"{assemblyName}.dll");
            var assembly = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, $@"{assemblyName}.dll"));

            lock (_migrationActions)
                _migrationActions.Add(assemblyName, migrationActionsList);

            return assembly;
        }

        static private void BuildMigrationClass(string assemblyName, ModuleBuilder moduleBuilder, int version)
        {
            var baseType = typeof(Migration);
            var typeBuilder = moduleBuilder.DefineType($"Migration{version}",
                TypeAttributes.Public | TypeAttributes.Class |
                TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout,
                baseType);

            var migAttrType = typeof(MigrationAttribute);
            var migAttrCtor = migAttrType.GetConstructor(new[] { typeof(long) });
            var attrBuilder = new CustomAttributeBuilder(migAttrCtor, new object[] { (long)version });
            typeBuilder.SetCustomAttribute(attrBuilder);

            var upBase = baseType.GetMethod("Up");
            var downBase = baseType.GetMethod("Down");

            BuildMigrationMethod(assemblyName, typeBuilder, upBase, version);
            BuildMigrationMethod(assemblyName, typeBuilder, downBase, version);

            typeBuilder.CreateType();
        }

        static private readonly MethodInfo _getMigrationActionsMethod = typeof(Migrator).GetMethod("GetMigrationAction");
        static private readonly MethodInfo _invokeMethod = typeof(Action<Migration>).GetMethod("Invoke");
        static private readonly Dictionary<string, MethodInfo> _getActionMethods = new Dictionary<string, MethodInfo> {
            { "Up", typeof(MigrationActions).GetProperty("Up").GetGetMethod() },
            { "Down", typeof(MigrationActions).GetProperty("Down").GetGetMethod() }
        };

        static private void BuildMigrationMethod(string assemblyName, TypeBuilder typeBuilder, MethodInfo baseMethod, int version)
        {
            var derivedMethod = typeBuilder.DefineMethod(
                baseMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(void),
                Type.EmptyTypes);

            var il = derivedMethod.GetILGenerator();
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldstr, assemblyName);
            il.Emit(OpCodes.Ldc_I4, version);
            il.Emit(OpCodes.Call, _getMigrationActionsMethod);
            il.Emit(OpCodes.Callvirt, _getActionMethods[baseMethod.Name]);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, _invokeMethod);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(derivedMethod, baseMethod);
        }
    }
}
