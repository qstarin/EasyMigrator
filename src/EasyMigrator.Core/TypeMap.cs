using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyMigrator.Parsing
{
    public interface ITypeMap
    {
        DbType this[FieldInfo field] { get; }
        void Add(Type type, DbType dbType);
        void Add(IEnumerable<Type> types, DbType dbType);
        void Add(IDictionary<Type, DbType> map);
        void Add(IDictionary<IEnumerable<Type>, DbType> map);
        void Add(Type type, Func<Type, FieldInfo, Context, DbType> dbTypeProvider);
        void Add(IEnumerable<Type> types, Func<Type, FieldInfo, Context, DbType> dbTypeProvider);
        void Add(IDictionary<Type, Func<Type, FieldInfo, Context, DbType>> providerMap);
        void Add(IDictionary<Type, Func<IEnumerable<Type>, FieldInfo, Context, DbType>> providerMap);
    }
}
