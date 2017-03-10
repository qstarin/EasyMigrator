using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using EasyMigrator.Tests.TableTest;


namespace EasyMigrator.Tests.Integration
{
    public interface IMigrationSet
    {
        IMigrationSet AddMigrationForTableTestCase(ITableTestCase tableTestCase);
        IMigrationSet AddMigrationForTableType(Type tableType);
        IMigrationSet AddMigrationForTableTypes(IEnumerable<Type> tableTypes);
        IMigrationSet AddMigrationForPocoDb(Action<NPoco.Database> up, Action<NPoco.Database> down);
        IMigrationSet AddMigrationForFluentMigrator(Action<global::FluentMigrator.Migration> up, Action<global::FluentMigrator.Migration> down);
        IMigrationSet AddMigrationForMigratorDotNet(Action<global::Migrator.Framework.Migration> up, Action<global::Migrator.Framework.Migration> down);
    }
}

namespace EasyMigrator.Tests.Integration.FluentMigrator
{
    public class MigrationSet : IMigrationSet
    {
        private readonly IList<MigrationActions> _migrationActions = new List<MigrationActions>();
        public IEnumerable<MigrationActions> MigrationActions => _migrationActions;

        public IMigrationSet AddMigrationForTableTestCase(ITableTestCase tableTestCase)
            => AddMigrationForTableTypes(tableTestCase.Datum.Select(d => d.Poco));

        public IMigrationSet AddMigrationForTableType(Type tableType)
            => AddMigrationForFluentMigrator(
                m => m.Create.Table(tableType),
                m => m.Delete.Table(tableType));

        public IMigrationSet AddMigrationForTableTypes(IEnumerable<Type> tableTypes)
            => AddMigrationForFluentMigrator(
                m => { foreach (var t in tableTypes) m.Create.Table(t); },
                m => { foreach (var t in tableTypes.Reverse()) m.Delete.Table(t); });

        public IMigrationSet AddMigrationForPocoDb(Action<NPoco.Database> up, Action<NPoco.Database> down)
            => AddMigrationForFluentMigrator(
                BuildNPocoMigrationAction(up),
                BuildNPocoMigrationAction(down));

        private Action<global::FluentMigrator.Migration> BuildNPocoMigrationAction(Action<NPoco.Database> action)
            => m => m.Execute.WithConnection((c, t) => {
                var db = new NPoco.Database(c as DbConnection);
                db.SetTransaction(t as DbTransaction);
                action(db);
            });

        public IMigrationSet AddMigrationForFluentMigrator(Action<global::FluentMigrator.Migration> up, Action<global::FluentMigrator.Migration> down)
        {
            _migrationActions.Add(new MigrationActions(up, down));
            return this;
        }

        public IMigrationSet AddMigrationForMigratorDotNet(Action<global::Migrator.Framework.Migration> up, Action<global::Migrator.Framework.Migration> down)
        {
            throw new InvalidOperationException("Cannot add a MigratorDotNet migration to a FluentMigrator migration set.");
        }
    }
}

namespace EasyMigrator.Tests.Integration.MigratorDotNet
{
    public class MigrationSet : IMigrationSet
    {
        private readonly IList<MigrationActions> _migrationActions = new List<MigrationActions>();
        public IEnumerable<MigrationActions> MigrationActions => _migrationActions;

        public IMigrationSet AddMigrationForTableTestCase(ITableTestCase tableTestCase)
            => AddMigrationForTableTypes(tableTestCase.Datum.Select(d => d.Poco));

        public IMigrationSet AddMigrationForTableType(Type tableType)
            => AddMigrationForMigratorDotNet(
                m => m.Database.AddTable(tableType),
                m => m.Database.RemoveTable(tableType));

        public IMigrationSet AddMigrationForTableTypes(IEnumerable<Type> tableTypes)
            => AddMigrationForMigratorDotNet(
                m => { foreach (var t in tableTypes) m.Database.AddTable(t); },
                m => { foreach (var t in tableTypes.Reverse()) m.Database.RemoveTable(t); });

        public IMigrationSet AddMigrationForPocoDb(Action<NPoco.Database> up, Action<NPoco.Database> down)
            => AddMigrationForMigratorDotNet(
                BuildNPocoMigrationAction(up),
                BuildNPocoMigrationAction(down));

        private Action<global::Migrator.Framework.Migration> BuildNPocoMigrationAction(Action<NPoco.Database> action)
            => m => {
                var cmd = m.Database.GetCommand();
                var db = new NPoco.Database(cmd.Connection as DbConnection);
                db.SetTransaction(cmd.Transaction as DbTransaction);
                action(db);
            };

        public IMigrationSet AddMigrationForFluentMigrator(Action<global::FluentMigrator.Migration> up, Action<global::FluentMigrator.Migration> down)
        {
            throw new InvalidOperationException("Cannot add a FluentMigrator migration to a MigratorDotNet migration set.");
        }

        public IMigrationSet AddMigrationForMigratorDotNet(Action<global::Migrator.Framework.Migration> up, Action<global::Migrator.Framework.Migration> down)
        {
            _migrationActions.Add(new MigrationActions(up, down));
            return this;
        }
    }
}