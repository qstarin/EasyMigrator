using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using EasyMigrator.Extensions;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;
using NPoco;


namespace EasyMigrator.Tests.Integration.FluentMigrator
{
    public class Migrator : MigratorBase<Migration>
    {
        private MigrationRunner Runner { get; }
        public Migrator(string connectionString) { Runner = GetRunner(connectionString); }

        protected override Action<Migration> GetDbActionMigration(Action<Database> action)
        {
            return m => m.Execute.WithConnection((c, t) => {
                var db = new Database(c as DbConnection);
                db.SetTransaction(t as DbTransaction);
                action(db);
            });
        }

        protected override Action<Migration> GetPocoMigration(Type poco, MigrationDirection direction)
        {
            if (direction == MigrationDirection.Up)
                return m => m.Create.Table(poco);
            else if (direction == MigrationDirection.Down)
                return m => m.Delete.Table(poco);
            else 
                return null;
        }

        public void Up(Action<Migration> action) { Runner.Up(new ActionMigration(action)); }
        public void Down(Action<Migration> action) { Runner.Down(new ActionMigration(action)); }
        protected override void Up(IEnumerable<Action<Migration>> actions) { Runner.Up(new ActionMigration(actions)); }
        protected override void Down(IEnumerable<Action<Migration>> actions) { Runner.Down(new ActionMigration(actions)); }

        private MigrationRunner GetRunner(string connectionString)
        {
            // http://stackoverflow.com/a/10508299/224087
            var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));
            var assembly = Assembly.GetExecutingAssembly();
            var migrationContext = new RunnerContext(announcer) { Namespace = GetType().Namespace };
            var options = new ProcessorOptions { PreviewOnly = false, Timeout = 60 };
            var factory = new SqlServer2012ProcessorFactory();
            var processor = factory.Create(connectionString, announcer, options);
            return new MigrationRunner(assembly, migrationContext, processor);
        }

        public class ActionMigration : Migration
        {
            private readonly IEnumerable<Action<Migration>> _up;
            private readonly IEnumerable<Action<Migration>> _down;
            public ActionMigration(Action<Migration> migration) : this(migration, migration) { }
            public ActionMigration(Action<Migration> up, Action<Migration> down) : this(new[] {up}, new[] {down}) { }
            public ActionMigration(IEnumerable<Action<Migration>> actions) : this(actions, actions) { }
            public ActionMigration(IEnumerable<Action<Migration>> up, IEnumerable<Action<Migration>> down) { _up = up; _down = down; }
            public override void Down() { _down.IfNotNull(ms => ms.ForEach(m => m(this))); }
            public override void Up() { _up.IfNotNull(ms => ms.ForEach(m => m(this))); }
        }
    }
}
