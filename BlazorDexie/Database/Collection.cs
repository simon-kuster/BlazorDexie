using BlazorDexie.JsInterop;

namespace BlazorDexie.Database
{
    public class Collection<T, TKey> : CollectionBase<T, TKey>
    {
        protected Collection()
        {
        }

        public Collection(Db db, string storeName, CollectionCommandExecuterJsInterop commandExecuterJsInterop)
        {
            Init(db, storeName, commandExecuterJsInterop);
        }

        protected override Collection<T, TKey> CreateNewColletion()
        {
            return this;
        }

        public virtual async Task<T[]> SortBy(string keyPath, CancellationToken cancellationToken = default)
        {
            return await Execute<T[]>("sortBy", cancellationToken, keyPath);
        }

        public virtual async Task<List<T>> SortByToList(string keyPath, CancellationToken cancellationToken = default)
        {
            return await Execute<List<T>>("sortBy", cancellationToken, keyPath);
        }
    }
}
