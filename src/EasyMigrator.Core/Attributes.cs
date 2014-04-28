using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator
{
    public class NameAttribute : Attribute
    {
        public string Name { get; set; }
        public NameAttribute(string name) { Name = name; }
    }

    public class PrimaryKeyAttribute : Attribute { }

    public class NullableAttribute : Attribute
    {
        public bool Nullable { get; set; }

        public NullableAttribute() : this(true) { }
        public NullableAttribute(bool nullable) { Nullable = nullable; }
    }

    public class NotNullableAttribute : NullableAttribute
    {
        public NotNullableAttribute() : base(false) { }
        public NotNullableAttribute(bool nullable) : base(!nullable) { }
    }

    public class ForeignKeyAttribute : Attribute
    {
        public string Table { get; private set; }
        public string Column { get; private set; }

        /// <summary>
        /// true by default
        /// </summary>
        public bool Indexed { get; set; }

        public ForeignKeyAttribute(string table)
        {
            this.Table = table;
            this.Column = table + "Id";
            this.Indexed = true;
        }
    }

    public class IndexedAttribute : Attribute
    {
        public bool Unique { get; set; }
    }

    public class UniqueAttribute : IndexedAttribute
    {
        public UniqueAttribute()
        {
            this.Unique = true;
        }
    }

    public class AnsiAttribute : Attribute { }

    public class LengthAttribute : Attribute
    {
        public int Length { get; private set; }

        public LengthAttribute(int length)
        {
            this.Length = length;
        }
    }

    public class FixedLengthAttribute : LengthAttribute
    {
        public FixedLengthAttribute(int length) : base(length) { }
    }

    public class MaxLengthAttribute : LengthAttribute
    {
        public const int MaximumLength = int.MaxValue;
        public MaxLengthAttribute() : base(MaximumLength) { }
    }

    public class DefaultAttribute : Attribute
    {
        public string Expression { get; private set; }

        public DefaultAttribute(string expression)
        {
            this.Expression = expression;
        }
    }
}
