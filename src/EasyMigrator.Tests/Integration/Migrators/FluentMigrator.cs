using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyMigrator.Extensions;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;


namespace EasyMigrator.Tests.Integration.Migrators
{
    public class FluentMigrator : IMigrator
    {
        public MigrationRunner Runner { get; set; }

        public FluentMigrator(string connectionString) { Runner = GetRunner(connectionString); }

        private Action<Migration> GetDbMigrationAction(Action<NPoco.Database> action)
        {
            return m => m.Execute.WithConnection((c, t) => {
                var db = new NPoco.Database(c);
                db.SetTransaction(t);
                action(db);
            });
        }

        public void Up(Action<NPoco.Database> action) { Up(GetDbMigrationAction(action)); }
        public void Down(Action<NPoco.Database> action) { Down(GetDbMigrationAction(action)); }
        public void Up(Type poco) { Up(m => m.Create.Table(poco)); }
        public void Down(Type poco) { Down(m => m.Delete.Table(poco)); }
        public void Up(Action<Migration> migration) { Runner.Up(new ActionMigration(migration)); }
        public void Down(Action<Migration> migration) { Runner.Down(new ActionMigration(migration)); }

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
            private readonly Action<Migration> _up;
            private readonly Action<Migration> _down;
            public ActionMigration(Action<Migration> migration) : this(migration, migration) { }
            public ActionMigration(Action<Migration> up, Action<Migration> down) { _up = up; _down = down; }
            public override void Down() { _down.IfNotNull(m => m(this)); }
            public override void Up() { _up.IfNotNull(m => m(this)); }
        }
    }
}
