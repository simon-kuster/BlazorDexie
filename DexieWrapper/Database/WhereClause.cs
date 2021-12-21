namespace DexieWrapper.Database
{
    public class WhereClause<T>
    {
        private readonly Collection<T> _collection;

        public WhereClause(Collection<T> collection)
        {
            _collection = collection;
        }

        public Collection<T> IsEqual(object key)
        {
            _collection.AddCommand("equals",  key);
            return _collection;
        }
    }
}
