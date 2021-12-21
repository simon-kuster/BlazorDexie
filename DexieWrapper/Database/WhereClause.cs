using DexieWrapper.JsInterop;

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
            _collection.CurrentCommand?.SubCommands.Add(new Command("equals", new List<object?> { key }));
            return _collection;
        }
    }
}
