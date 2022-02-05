namespace Nosthy.Blazor.DexieWrapper.Database
{
    public class WhereClause<T, TKey>
    {
        private readonly Collection<T, TKey> _collection;

        public WhereClause(Collection<T, TKey> collection)
        {
            _collection = collection;
        }

        public Collection<T, TKey> AnyOf(params object[] keys)
        {
            _collection.AddCommand("anyOf", keys);
            return _collection;
        }

        public Collection<T, TKey> IsEqual(object key)
        {
            _collection.AddCommand("equals", key);
            return _collection;
        }
    }
}
