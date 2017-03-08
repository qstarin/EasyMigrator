using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using EasyMigrator.Extensions;
using Migrator.Framework;
using NPoco;


namespace EasyMigrator.Tests.Integration.Migrators
{
    public class MigratorDotNet : MigratorBase<Migration>
    {
        private readonly string _connectionString;
        public MigratorDotNet(string connectionString) { _connectionString = connectionString; }

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

        private Migrator.Migrator BuildRunner(string connectionString, IEnumerable<Action<Migration>> actions)
            => new Migrator.Migrator("SqlServer", connectionString, BuildMigrationAssembly(actions));

        static private readonly ConcurrentDictionary<string, Tuple<Action<Migration>, Action<Migration>>> _migrationActions = new ConcurrentDictionary<string, Tuple<Action<Migration>, Action<Migration>>>();
        static public Action<Migration> GetMigrationAction(string assemblyName, MigrationDirection migrationDirection)
        {
            if (!_migrationActions.ContainsKey(assemblyName))
                return null;

            var assemblyActions = _migrationActions[assemblyName];
            if (migrationDirection == MigrationDirection.Up)
                return assemblyActions.Item1;
            if (migrationDirection == MigrationDirection.Down)
                return assemblyActions.Item2;

            return null;
        }

        private Assembly BuildMigrationAssembly(IEnumerable<Action<Migration>> actions)
        {
            var assemblyName = $"mdn_test_{Guid.NewGuid()}";
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name, assemblyName + ".dll");
            BuildDirectMigrationClass(assemblyName, moduleBuilder, 1, actions);

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dynamicDir = AppDomain.CurrentDomain.DynamicDirectory;
            var thisAssembly = typeof(MigratorDotNet).Assembly;
            string thisAssemblyLocation = thisAssembly.Location;
            var curDir = Environment.CurrentDirectory;

            assemblyBuilder.Save($@"{assemblyName}.dll");
            var assembly = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, $@"{assemblyName}.dll"));

            Action<Migration> combinedAction = m => { foreach (var a in actions) a(m); };
            _migrationActions.TryAdd(assemblyName, new Tuple<Action<Migration>, Action<Migration>>(combinedAction, combinedAction));

            return assembly;
        }

        private void BuildDirectMigrationClass(string assemblyName, ModuleBuilder moduleBuilder, long version, IEnumerable<Action<Migration>> actions)
        {
            var baseType = typeof(Migration);
            var typeBuilder = moduleBuilder.DefineType($"Migration{version}",
                TypeAttributes.Public | TypeAttributes.Class |
                TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout,
                baseType);

            var migAttrType = typeof(MigrationAttribute);
            var migAttrCtor = migAttrType.GetConstructor(new[] { typeof(long) });
            var attrBuilder = new CustomAttributeBuilder(migAttrCtor, new object[] { version });
            typeBuilder.SetCustomAttribute(attrBuilder);

            var upBase = baseType.GetMethod("Up");
            var downBase = baseType.GetMethod("Down");

            var getMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });
            var getActionMethod = typeof(MigratorDotNet).GetMethod("GetMigrationAction");
            var invokeMethod = typeof(Action<Migration>).GetMethod("Invoke");

            var up = typeBuilder.DefineMethod(
                upBase.Name, 
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, 
                typeof(void), 
                Type.EmptyTypes);

            Action<Migration> combinedAction = m => { foreach (var a in actions) a(m); };
            
            var upIl = up.GetILGenerator();
            upIl.Emit(OpCodes.Nop);
            upIl.Emit(OpCodes.Ldstr, assemblyName);
            upIl.Emit(OpCodes.Ldc_I4_0);
            upIl.Emit(OpCodes.Call, getActionMethod);
            upIl.Emit(OpCodes.Ldarg_0);
            upIl.Emit(OpCodes.Callvirt, invokeMethod);
            upIl.Emit(OpCodes.Nop);
            upIl.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(up, upBase);


            var down = typeBuilder.DefineMethod(
                downBase.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(void),
                Type.EmptyTypes);


            var downIl = down.GetILGenerator();
            downIl.Emit(OpCodes.Nop);
            downIl.Emit(OpCodes.Ldstr, moduleBuilder.Name);
            downIl.Emit(OpCodes.Ldc_I4_1);
            downIl.Emit(OpCodes.Call, getActionMethod);
            downIl.Emit(OpCodes.Ldarg_0);
            downIl.Emit(OpCodes.Callvirt, invokeMethod);
            downIl.Emit(OpCodes.Nop);
            downIl.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(down, downBase);

            typeBuilder.CreateType();
        }

        public class ActionMigration : Migration
        {
            private readonly IEnumerable<Action<Migration>> _up;
            private readonly IEnumerable<Action<Migration>> _down;
            public ActionMigration(Action<Migration> migration) : this(migration, migration) { }
            public ActionMigration(Action<Migration> up, Action<Migration> down) : this(new[] { up }, new[] { down }) { }
            public ActionMigration(IEnumerable<Action<Migration>> actions) : this(actions, actions) { }
            public ActionMigration(IEnumerable<Action<Migration>> up, IEnumerable<Action<Migration>> down) { _up = up; _down = down; }
            public override void Down() => _down?.ForEach(m => m(this));
            public override void Up() => _up?.ForEach(m => m(this));
        }
    }
}
