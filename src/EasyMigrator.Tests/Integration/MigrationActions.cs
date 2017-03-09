using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EasyMigrator.Tests.Integration
{
    public class MigrationActions<TMigration>
    {
        public MigrationActions(Action<TMigration> upAndDown) : this(upAndDown, upAndDown) { }

        public MigrationActions(Action<TMigration> up, Action<TMigration> down)
        {
            Up = up;
            Down = down;
        }

        public Action<TMigration> Up { get; }
        public Action<TMigration> Down { get; }
    }
}


namespace EasyMigrator.Tests.Integration.MigratorDotNet
{
    public class MigrationActions : MigrationActions<global::Migrator.Framework.Migration>
    {
        public MigrationActions(Action<global::Migrator.Framework.Migration> upAndDown) : base(upAndDown) { }
        public MigrationActions(Action<global::Migrator.Framework.Migration> up, Action<global::Migrator.Framework.Migration> down) : base(up, down) { }
    }
}


namespace EasyMigrator.Tests.Integration.FluentMigrator
{
    public class MigrationActions : MigrationActions<global::FluentMigrator.Migration>
    {
        public MigrationActions(Action<global::FluentMigrator.Migration> upAndDown) : base(upAndDown) { }
        public MigrationActions(Action<global::FluentMigrator.Migration> up, Action<global::FluentMigrator.Migration> down) : base(up, down) { }
    }
}
