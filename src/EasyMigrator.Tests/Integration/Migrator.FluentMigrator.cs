using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;


namespace EasyMigrator.Tests.Integration.FluentMigrator
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
                throw new InvalidOperationException("Migration set is not a FluentMigrator migration set.");

            return new CompiledMigrations(new List<Migration>(set.MigrationActions.Select(a => new ActionMigration(a))));
        }

        public void Up(ICompiledMigrations migrations)
        {
            var runner = BuildRunner(_connectionString);
            foreach (var m in GetMigrations(migrations))
                runner.Up(m);
        }

        public void Down(ICompiledMigrations migrations)
        {
            var runner = BuildRunner(_connectionString);
            foreach (var m in GetMigrations(migrations).Reverse())
                runner.Down(m);
        }

        private IList<Migration> GetMigrations(ICompiledMigrations migrations)
        {
            var mig = migrations as CompiledMigrations;
            if (mig == null)
                throw new InvalidOperationException("Compiled migrations are not for FluentMigrator.");

            return mig.Migrations;
        }

        private MigrationRunner BuildRunner(string connectionString)
        {
            // http://stackoverflow.com/a/10508299/224087
            var announcer = new TextWriterAnnouncer(s => Console.Out.WriteLine(s));
            var assembly = Assembly.GetExecutingAssembly();
            var migrationContext = new RunnerContext(announcer) { Namespace = GetType().Namespace, TransactionPerSession = true };
            var options = new ProcessorOptions { PreviewOnly = false, Timeout = 60 };
            var factory = new SqlServer2012ProcessorFactory();
            var processor = factory.Create(connectionString, announcer, options);
            return new MigrationRunner(assembly, migrationContext, processor);
        }

        private class ActionMigration : Migration
        {
            private readonly MigrationActions _actions;
            public ActionMigration(MigrationActions actions) { _actions = actions; }

            public override void Up() => _actions.Up(this);
            public override void Down() => _actions.Down(this);
        }
    }
}
