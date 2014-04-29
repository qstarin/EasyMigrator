using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyMigrator.Extensions
{
    static public class ReflectionExtensions
    {
        public static TAttr GetAttribute<TAttr>(this MemberInfo member) where TAttr : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttr), false).Cast<TAttr>().FirstOrDefault();
        }

        public static bool HasAttribute<TAttr>(this MemberInfo member) where TAttr : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttr), false).Length > 0;
        }

        public static bool IsNullableType(this Type type)
        {
            // http://msdn.microsoft.com/en-us/library/ms366789.aspx
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static bool IsNumeric(this Type type)
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
    }
}
