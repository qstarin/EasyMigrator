using System;
using System.Collections.Generic;
using System.Data.Common;
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

        private Assembly BuildMigrationAssembly(IEnumerable<Action<Migration>> actions)
        {
            var assemblyName = $"mdn_test_{Guid.NewGuid()}";
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name, assemblyName + ".dll");
            BuildMigrationClass(moduleBuilder, 1, actions);

            return assemblyBuilder;
        }

        private void BuildMigrationClass(ModuleBuilder moduleBuilder, long version, IEnumerable<Action<Migration>> actions)
        {
            var baseType = typeof(ActionMigration);
            var typeBuilder = moduleBuilder.DefineType($"Migration{version}",
                TypeAttributes.Public | TypeAttributes.Class |
                TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout,
                baseType);

            var migAttrType = typeof(MigrationAttribute);
            var migAttrCtor = migAttrType.GetConstructor(new[] { typeof(long) });
            typeBuilder.SetCustomAttribute(migAttrCtor, BitConverter.GetBytes(version));

            var baseCtor = baseType.GetConstructor(new[] { typeof(IEnumerable<Action<Migration>>) });
            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            var ilg = ctor.GetILGenerator();
            ilg.Emit(OpCodes.Ldarg_0);
            //ilg.Emit(OpCodes.);
            ilg.Emit(OpCodes.Call, baseCtor);
            ilg.Emit(OpCodes.Nop);
            ilg.Emit(OpCodes.Nop);
            ilg.Emit(OpCodes.Ret);
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
