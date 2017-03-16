using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace EasyMigrator
{
    // TODO: Column & Table name attributes, multi-column indices (sorted..)
    [Obsolete("This attribute is for legacy code that is migrating. Use object types or nullable value types for a nullable field, and use NotNullAttribute to override object types for a non-nullable field")]
    public class NullAttribute : Attribute { }
    public class NotNullAttribute : Attribute { }

    public class NameAttribute : Attribute
    {
        public string Name { get; }
        public NameAttribute(string name) { Name = name; }
    }

    public class DbTypeAttribute : Attribute
    {
        public DbType DbType { get; }
        public DbTypeAttribute(DbType dbType) { DbType = dbType; }
    }

    public class NoPkAttribute : Attribute { }

    public class PkAttribute : Attribute
    {
        public string Name { get; }
        public bool Clustered { get; set; } = true;

        public PkAttribute() { }
        public PkAttribute(string name) { Name = name; }
    }

    public class AutoIncAttribute : Attribute, Parsing.Model.IAutoIncrement
    {
        public long Seed { get; }
        public long Step { get; }

        public AutoIncAttribute() : this(1) { }
        public AutoIncAttribute(long seed) : this(seed, 1) { }
        public AutoIncAttribute(long seed, long step) { Seed = seed; Step = step; }
    }

    public class PrecisionAttribute : Attribute, Parsing.Model.IPrecision
    {
        internal Length? DefinedPrecision { get; }
        public int Precision { get; }
        public int Scale { get; }

        public PrecisionAttribute(int precision, int scale)
            : this(scale) { Precision = precision; }

        public PrecisionAttribute(Length precision, int scale)
            : this(scale) { DefinedPrecision = precision; }

        private PrecisionAttribute(int scale) { Scale = scale; }
    }

    public class FkAttribute : Attribute, Parsing.Model.IForeignKey
    {
        public string Name { get; set; }
        public string Table { get; }
        public Type TableType { get; }
        public string Column { get; set; }
        public bool? Indexed { get; set; }

        public FkAttribute(string table) { Table = table; }
        public FkAttribute(Type tableType) { TableType = tableType; }
    }

    public class IndexAttribute : Attribute, Parsing.Model.IIndex
    {
        public string Name { get; set; }
        public bool Unique { get; protected set; }
        public bool Clustered { get; protected set; }
    }

    public class UniqueAttribute : IndexAttribute
    {
        public UniqueAttribute() { Unique = true; }
    }

    public class ClusteredIndexAttribute : UniqueAttribute
    {
        public ClusteredIndexAttribute() { Clustered = true; }
    }

    public class AnsiAttribute : Attribute { }

    public enum Length { Default, Short, Medium, Long, Max }

    public class LengthAttribute : Attribute
    {
        internal Length? DefinedLength { get; }
        public int Length { get; }

        public LengthAttribute(int length) { Length = length; }
        public LengthAttribute(Length length) { DefinedLength = length; }
    }

    public class ShortAttribute : LengthAttribute { public ShortAttribute() : base(EasyMigrator.Length.Short) { } }
    public class MediumAttribute : LengthAttribute { public MediumAttribute() : base(EasyMigrator.Length.Medium) { } }
    public class LongAttribute : LengthAttribute { public LongAttribute() : base(EasyMigrator.Length.Long) { } }
    public class MaxAttribute : LengthAttribute { public MaxAttribute() : base(EasyMigrator.Length.Max) { } }

    public class FixedAttribute : LengthAttribute
    {
        public FixedAttribute(int length) : base(length) { }
    }

    public class DefaultAttribute : Attribute
    {
        public string Expression { get; }
        public DefaultAttribute(string expression) { Expression = expression; }
    }
}
