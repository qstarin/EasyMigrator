using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Migrator.Framework;
using NPoco;


namespace EasyMigrator.Tests.Integration.MigratorDotNet
{
    public class Migrator : MigratorBase<Migration>
    {
        private readonly string _connectionString;
        public Migrator(string connectionString) { _connectionString = connectionString; }

        protected override Action<Migration> GetDbActionMigration(Action<Database> action)
        {
            return m => {
                var cmd = m.Database.GetCommand();
                var db = new Database(cmd.Connection as DbConnection);
                db.SetTransaction(cmd.Transaction as DbTransaction);
                action(db);
            };
        }

        protected override Action<Migration> GetPocoMigration(Type poco, MigrationDirection direction)
        {
            if (direction == MigrationDirection.Up)
                return m => m.Database.AddTable(poco);
            else if (direction == MigrationDirection.Down)
                return m => m.Database.RemoveTable(poco);
            else
                return null;
        }

        public void Up(Action<Migration> action) => Up(new[] { action });
        public void Down(Action<Migration> action) => Down(new[] { action });

        protected override void Up(IEnumerable<Action<Migration>> actions)
            => BuildRunner(_connectionString, actions).MigrateToLastVersion();

        protected override void Down(IEnumerable<Action<Migration>> actions)
            => BuildRunner(_connectionString, actions).MigrateTo(0);

        public override IMigrationSet CreateMigrationSet() => new MigrationSet();

        private global::Migrator.Migrator BuildRunner(string connectionString, IEnumerable<Action<Migration>> actions)
            => new global::Migrator.Migrator("SqlServer", connectionString, BuildMigrationAssembly(new List<MigrationActions> {
                new MigrationActions(m => { foreach (var a in actions) a(m); })
            }));

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
