using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyMigrator.Extensions
{
    static public class ReflectionExtensions
    {
        static public TAttr GetAttribute<TAttr>(this MemberInfo member) where TAttr : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttr), false).Cast<TAttr>().FirstOrDefault();
        }

        static public bool HasAttribute<TAttr>(this MemberInfo member) where TAttr : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttr), false).Length > 0;
        }

        static public bool IsNullableType(this Type type)
        {
            // http://msdn.microsoft.com/en-us/library/ms366789.aspx
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        static public bool IsNumeric(this Type type)
        {
            if (type.IsNullableType())
                type = type.GetGenericArguments()[0];

            switch (Type.GetTypeCode(type)) {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        static public bool InheritsFrom<T>(this Type type) { return typeof(T).IsAssignableFrom(type); }

        /// <summary>
        /// lets you do: type.InheritsFrom(typeof(Something&lt;&gt;))
        /// </summary>
        static public bool InheritsFrom(this Type type, Type superType)
        {
            if (!type.IsClass && !type.IsInterface)
                return false;

            if (!superType.IsGenericTypeDefinition)
                return superType.IsAssignableFrom(type);

            // http://stackoverflow.com/questions/457676/c-reflection-check-if-a-class-is-derived-from-a-generic-class
            for (var t = type; t != null && t != typeof(object); t = t.BaseType) {
                if (t.IsGenericType)
                    t = t.GetGenericTypeDefinition();
                if (t == superType)
                    return true;
            }

            return false;
        }
    }
}
