using Nosthy.Blazor.DexieWrapper.JsInterop;
using Nosthy.Blazor.DexieWrapper.Utils;
using System.Dynamic;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public class Store<T, TKey> : Collection<T, TKey>, IStore
    {
        public string[] Indices { get; }

        protected override List<Command> CurrentCommands { get => new List<Command>(); }

        public Store(params string[] indices)
        {
            Indices = indices.Select(Camelizer.ToCamelCase).ToArray();
        }

        public async Task<TKey> Add(T item)
        {
            return await Execute<TKey>("add", item);
        }

        public async Task<TKey> Add(T item, TKey? key)
        {
            return await Execute<TKey>("add", item, key);
        }

        public async Task<TKey> BulkAdd(T[] items)
        {
            return await Execute<TKey>("bulkAdd", items);
        }

        public async Task BulkAdd(T[] items, IEnumerable<TKey>? keys)
        {
            await ExecuteNonQuery("bulkAdd", items, keys);
        }

        public async Task<List<TKey>> BulkAddReturnAllKeys(T[] items)
        {
            return await Execute<List<TKey>>("bulkAdd", items, new { allKeys = true });
        }

        public async Task BulkDelete(IEnumerable<TKey> primaryKeys)
        {
            await ExecuteNonQuery("bulkDelete", primaryKeys);
        }

        public async Task<T?[]> BulkGet(IEnumerable<TKey> keys)
        {
            return await Execute<T[]>("bulkGet", keys);
        }

        public async Task<TKey> BulkPut(T[] items)
        {
            return await Execute<TKey>("bulkPut", items);
        }

        public async Task BulkPut(T[] items, IEnumerable<TKey>? keys)
        {
            await ExecuteNonQuery("bulkPut", items, keys);
        }

        public async Task<List<TKey>> BulkPutReturnAllKeys(T[] items)
        {
            return await Execute<List<TKey>>("bulkPut", items, new { allKeys = true });
        }

        public async Task Delete(TKey primaryKey)
        {
            await ExecuteNonQuery("delete", primaryKey);
        }

        public async Task<T?> Get(TKey primaryKey)
        {
            return await Execute<T?>("get", primaryKey);
        }

        public async Task<TKey> Put(T item)
        {
            return await Execute<TKey>("put", item);
        }

        public async Task<TKey> Put(T item, TKey? key)
        {
            return await Execute<TKey>("put", item, key);
        }

        public async Task<int> Update(TKey key, Dictionary<string, object> changes)
        {
            var changesObject = new ExpandoObject();
            foreach (var change in changes)
            {
                changesObject.TryAdd(Camelizer.ToCamelCase(change.Key), change.Value);
            }

            return await Execute<int>("update", key, changesObject);
        }

        public WhereClause<T, TKey> Where(string indexOrPrimaryKey)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("where", Camelizer.ToCamelCase(indexOrPrimaryKey));
            return new WhereClause<T, TKey>(collection);
        }

        public Collection<T, TKey> Where(Dictionary<string, object> criterias)
        {
            var collection = CreateNewColletion();

            var criteriaObject = new ExpandoObject();
            foreach (var criteria in criterias)
            {
                criteriaObject.TryAdd(Camelizer.ToCamelCase(criteria.Key), criteria.Value);
            }

            collection.AddCommand("where", criteriaObject);

            return collection;
        }

        public async Task Clear()
        {
            await ExecuteNonQuery("clear");
        }

        protected override Collection<T, TKey> CreateNewColletion()
        {
            return new Collection<T, TKey>(Db, StoreName, CommandExecuterJsInterop);
        }
    }
}
