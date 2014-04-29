using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EasyMigrator
{
    public class NotNullAttribute : Attribute { }

    public class PkAttribute : Attribute { }

    public class AutoIncAttribute : Attribute, Model.IAutoIncrement
    {
        public long? Seed { get; set; }
        public long? Step { get; set; }
    }

    public class PrecisionAttribute : Attribute, Model.IPrecision
    {
        public int? Scale { get; private set; }
        public int? Precision { get; private set; }

        public PrecisionAttribute(int scale) { Scale = scale; }

        public PrecisionAttribute(int scale, int precision)
            : this(scale) { Precision = precision; }
    }

    public class FkAttribute : Attribute, Model.IForeignKey
    {
        public string Name { get; set; }
        public string Table { get; private set; }
        public string Column { get; set; }
        public bool Indexed { get; set; }

        public FkAttribute(string table)
        {
            Table = table;
            Column = "Id"; // TODO: Need to account for different conventions
            Indexed = true;
        }
    }

    public class IndexAttribute : Attribute, Model.IIndex
    {
        public string Name { get; set; }
        public bool Unique { get; set; }
        public bool Clustered { get; set; }
    }

    public class UniqueAttribute : IndexAttribute
    {
        public UniqueAttribute() { Unique = true; }
    }

    public class AnsiAttribute : Attribute { }

    public enum Length { Short, Medium, Long, Max }

    public class LengthAttribute : Attribute
    {
        public Length? DefinedLength { get; private set; }
        public int Length { get; private set; }

        public LengthAttribute(int length) { Length = length; }
        public LengthAttribute(Length length) { DefinedLength = length; }
    }

    public class ShortAttribute : LengthAttribute { public ShortAttribute() : base(EasyMigrator.Length.Short) { } }
    public class MediumAttribute : LengthAttribute { public MediumAttribute() : base(EasyMigrator.Length.Medium) { } }
    public class LongAttribute : LengthAttribute { public LongAttribute() : base(EasyMigrator.Length.Long) { } }
    public class MaxAttribute : LengthAttribute { public MaxAttribute() : base(EasyMigrator.Length.Max) { } }

    public class FixedLengthAttribute : LengthAttribute
    {
        public FixedLengthAttribute(int length) : base(length) { }
    }

    public class DefaultAttribute : Attribute
    {
        public string Expression { get; private set; }
        public DefaultAttribute(string expression) { Expression = expression; }
    }
}
