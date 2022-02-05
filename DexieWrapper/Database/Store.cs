using DexieWrapper.JsInterop;
using DexieWrapper.Utils;
using System.Dynamic;

namespace DexieWrapper.Database
{
    public class Store<T, TKey> : Collection<T, TKey>, IStore
    {
        public string[] Indices { get; }

        protected override List<Command> CurrentCommands { get => new List<Command>(); }

        public Store(params string[] indices)
        {
            Indices = indices.Select(Camelizer.ToCamelCase).ToArray();
        }

        public async Task<T?> Get(TKey primaryKey)
        {
            return await Execute<T?>("get", primaryKey);
        }

        public async Task<T?[]> BulkGet(IEnumerable<TKey> keys)
        {
            return await Execute<T[]>("bulkGet", keys);
        }

        public async Task<TKey> Put(T item)
        {
            return await Execute<TKey>("put", item);
        }

        public async Task<TKey> Put(T item, TKey? key)
        {
            return await Execute<TKey>("put", item, key);
        }

        public async Task BulkPut(T[] items, IEnumerable<TKey>? keys = null)
        {
            await ExecuteNonQuery("bulkPut", items, keys);
        }

        public async Task Delete(TKey primaryKey)
        {
            await ExecuteNonQuery("delete", primaryKey);
        }

        public async Task BulkDelete(IEnumerable<TKey> primaryKeys)
        {
            await ExecuteNonQuery("bulkDelete", primaryKeys);
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

            var obj = new ExpandoObject();
            foreach (var criteria in criterias)
            {
                obj.TryAdd(Camelizer.ToCamelCase(criteria.Key), criteria.Value);
            }

            collection.AddCommand("where", obj);

            return collection;
        }

        public async Task Clear()
        {
            await ExecuteNonQuery("clear");
        }

        protected override Collection<T, TKey> CreateNewColletion()
        {
            return new Collection<T,TKey>(Db, StoreName, CommandExecuterJsInterop);
        }
    }
}
