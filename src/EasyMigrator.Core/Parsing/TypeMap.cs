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
        ITypeMap Add(Type underlyingType, DbType dbType);
        ITypeMap Add(IEnumerable<Type> underlyingTypes, DbType dbType);
        ITypeMap Add(IDictionary<Type, DbType> map);
        ITypeMap Add(IDictionary<IEnumerable<Type>, DbType> map);
        ITypeMap Add(Type underlyingType, Func<FieldInfo, DbType> dbTypeProvider);
        ITypeMap Add(IEnumerable<Type> underlyingTypes, Func<FieldInfo, DbType> dbTypeProvider);
        ITypeMap Add(IDictionary<Type, Func<FieldInfo, DbType>> providerMap);
        ITypeMap Add(IDictionary<IEnumerable<Type>, Func<FieldInfo, DbType>> providerMap);
    }

    public class TypeMap : ITypeMap
    {
        private class ProviderPair
        {
            private readonly DbType _dbType;
            private readonly Func<FieldInfo, DbType> _dbTypeProvider = null;

            public ProviderPair(DbType dbType) { _dbType = dbType; }
            public ProviderPair(Func<FieldInfo, DbType> dbTypeProvider) { _dbTypeProvider = dbTypeProvider; }

            public DbType GetDbType(FieldInfo field) { return _dbTypeProvider == null ? _dbType : _dbTypeProvider(field); }
        }

        private readonly Dictionary<Type, ProviderPair> _map = new Dictionary<Type, ProviderPair>();
        public DbType this[FieldInfo field]
        {
            get {
                var type = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
                if (!_map.ContainsKey(type))
                    throw new Exception("No DbType mapped to native type " + type.Name);

                return _map[type].GetDbType(field);
            }
        }

        public ITypeMap Add(Type underlyingType, DbType dbType) { Add(underlyingType, new ProviderPair(dbType)); return this; }
        public ITypeMap Add(IEnumerable<Type> underlyingTypes, DbType dbType) { foreach (var underlyingType in underlyingTypes) Add(underlyingType, dbType); return this; }
        public ITypeMap Add(IDictionary<Type, DbType> map) { foreach (var kv in map) Add(kv.Key, kv.Value); return this; }
        public ITypeMap Add(IDictionary<IEnumerable<Type>, DbType> map) { foreach (var kv in map) Add(kv.Key, kv.Value); return this; }
        public ITypeMap Add(Type underlyingType, Func<FieldInfo, DbType> dbTypeProvider) { Add(underlyingType, new ProviderPair(dbTypeProvider)); return this; }
        public ITypeMap Add(IEnumerable<Type> underlyingTypes, Func<FieldInfo, DbType> dbTypeProvider) { foreach (var underlyingType in underlyingTypes) Add(underlyingType, dbTypeProvider); return this; }
        public ITypeMap Add(IDictionary<Type, Func<FieldInfo, DbType>> providerMap) { foreach (var kv in providerMap) Add(kv.Key, kv.Value); return this; }
        public ITypeMap Add(IDictionary<IEnumerable<Type>, Func<FieldInfo, DbType>> providerMap) { foreach (var kv in providerMap) Add(kv.Key, kv.Value); return this; }

        private void Add(Type underlyingType, ProviderPair providerPair)
        {
            if (_map.ContainsKey(underlyingType))
                throw new Exception("Native type '" + underlyingType.Name + "' is already mapped.");
            _map.Add(underlyingType, providerPair);
        }
    }
}
