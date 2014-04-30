using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public void Up(Type poco)
        {
            GetRunner().Up(new ActionMigration(m => m.Create.Table(poco)));
        }

        public MigrationRunner GetRunner()
        {
            // http://stackoverflow.com/a/10508299/224087
            var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));
            var assembly = Assembly.GetExecutingAssembly();
            var migrationContext = new RunnerContext(announcer) { Namespace = GetType().Namespace };
            var options = new ProcessorOptions { PreviewOnly = false, Timeout = 60 };
            var factory = new SqlServer2012ProcessorFactory();
            var processor = factory.Create(IntegrationTestBase.ConnectionString, announcer, options);
            return new MigrationRunner(assembly, migrationContext, processor);
        }

        public class ActionMigration : Migration
        {
            private readonly Action<Migration> _migration;
            public ActionMigration(Action<Migration> migration) { _migration = migration; }
            public override void Down() { }
            public override void Up() { _migration(this); }
        }
    }
}
