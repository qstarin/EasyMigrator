using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EasyMigrator
{
    public class PrimaryKeyAttribute : Attribute {}

    public class AutoIncrementAttribute : Attribute, Model.IAutoIncrement
    {
        public long? Seed { get; set; }
        public long? Step { get; set; }
    }

    public class PrecisionAttribute : Attribute, Model.IPrecision
    {
        public byte? Scale { get; set; }
        public byte? Precision { get; set; }
    }

    public class NullableAttribute : Attribute
    {
        public bool Nullable { get; set; }

        public NullableAttribute() : this(true) { }
        public NullableAttribute(bool nullable) { Nullable = nullable; }
    }

    public class NotNullableAttribute : NullableAttribute
    {
        public NotNullableAttribute() : base(false) { }
    }

    public class ForeignKeyAttribute : Attribute, Model.IForeignKey
    {
        public string Name { get; set; }
        public string Table { get; private set; }
        public string Column { get; set; }
        public bool Indexed { get; set; }

        public ForeignKeyAttribute(string table)
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

    public enum Lengths { Small, Medium, Large, Max }

    public class LengthAttribute : Attribute
    {
        public Lengths? LengthEnum { get; private set; }
        public int Length { get; private set; }

        public LengthAttribute(int length) { Length = length; }
        public LengthAttribute(Lengths length) { LengthEnum = length; }
    }

    public class MaxLengthAttribute : LengthAttribute
    {
        public MaxLengthAttribute() : base(Lengths.Max) { }
    }

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
