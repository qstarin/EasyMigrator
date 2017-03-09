using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;


namespace EasyMigrator.Tests.Integration
{
    abstract public class MigratorBase<TMigrationBase> : IMigrator
    {
        public void Up(Type poco) { Up(new[] { poco }); }
        public void Up(IEnumerable<Type> pocos) { Up(pocos.Select(p => GetPocoMigration(p, MigrationDirection.Up))); }
        public void Up(Action<Database> action) { Up(new[] { action }); }
        public void Up(IEnumerable<Action<Database>> actions) { Up(actions.Select(GetDbActionMigration)); }
        public void Down(Type poco) { Down(new[] { poco }); }
        public void Down(IEnumerable<Type> pocos) { Down(pocos.Select(p => GetPocoMigration(p, MigrationDirection.Down))); }
        public void Down(Action<Database> action) { Down(new[] { action }); }
        public void Down(IEnumerable<Action<Database>> actions) { Down(actions.Select(GetDbActionMigration)); }

        abstract protected void Up(IEnumerable<Action<TMigrationBase>> actions);
        abstract protected void Down(IEnumerable<Action<TMigrationBase>> actions);
        abstract protected Action<TMigrationBase> GetDbActionMigration(Action<Database> action);
        abstract protected Action<TMigrationBase> GetPocoMigration(Type poco, MigrationDirection direction);
    }
}
