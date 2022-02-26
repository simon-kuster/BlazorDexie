namespace Nosthy.Blazor.DexieWrapper.Database
{
    public class WhereClause<T, TKey>
    {
        private readonly Collection<T, TKey> _collection;

        public WhereClause(Collection<T, TKey> collection)
        {
            _collection = collection;
        }

        public Collection<T, TKey> Above(object value)
        {
            _collection.AddCommand("above", value);
            return _collection;
        }

        public Collection<T, TKey> AboveOrEqual(object value)
        {
            _collection.AddCommand("aboveOrEqual", value);
            return _collection;
        }

        public Collection<T, TKey> AnyOf(params object[] keys)
        {
            _collection.AddCommand("anyOf", keys);
            return _collection;
        }

        public Collection<T, TKey> AnyOfIgnoreCase(params object[] keys)
        {
            _collection.AddCommand("anyOfIgnoreCase", keys);
            return _collection;
        }

        public Collection<T, TKey> Below(object value)
        {
            _collection.AddCommand("below", value);
            return _collection;
        }

        public Collection<T, TKey> BelowOrEqual(object value)
        {
            _collection.AddCommand("belowOrEqual", value);
            return _collection;
        }

        public Collection<T, TKey> Between(object minValue, object maxValue, bool includeMin = true, bool includeMax = false)
        {
            _collection.AddCommand("between", minValue, maxValue, includeMin, includeMax);
            return _collection;
        }

        public Collection<T, TKey> IsEqual(object value)
        {
            _collection.AddCommand("equals", value);
            return _collection;
        }

        public Collection<T, TKey> EqualIgnoreCase(object value)
        {
            _collection.AddCommand("equalsIgnoreCase", value);
            return _collection;
        }

        public Collection<T, TKey> InAnyRange(object[][] ranges, bool includeMin = true, bool includeMax = false)
        {
            object options = new { includeLowers = includeMin, includeUppers = includeMax };

            _collection.AddCommand("inAnyRange", ranges, options);
            return _collection;
        }

        public Collection<T, TKey> NoneOf(params object[] keys)
        {
            _collection.AddCommand("noneOf", keys);
            return _collection;
        }

        public Collection<T, TKey> NotEqual(object value)
        {
            _collection.AddCommand("notEqual", value);
            return _collection;
        }

        public Collection<T, TKey> StartsWith(string prefix)
        {
            _collection.AddCommand("startsWith", prefix);
            return _collection;
        }

        public Collection<T, TKey> StartsWithAnyOf(params string[] prefixes)
        {
            _collection.AddCommand("startsWithAnyOf", prefixes);
            return _collection;
        }

        public Collection<T, TKey> StartsWithAnyOfIgnoreCase(params string[] prefixes)
        {
            _collection.AddCommand("startsWithAnyOfIgnoreCase", prefixes);
            return _collection;
        }

        public Collection<T, TKey> StartsWithIgnoreCase(string prefix)
        {
            _collection.AddCommand("startsWithIgnoreCase", prefix);
            return _collection;
        }
    }
}
