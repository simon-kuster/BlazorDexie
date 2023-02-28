using Nosthy.Blazor.DexieWrapper.JsInterop;
using Nosthy.Blazor.DexieWrapper.Utils;
using System.Dynamic;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public class Store<T, TKey> : Collection<T, TKey>, IStore
    {
        public string[] SchemaDefinitions { get; }
        public string PrimaryKey { get; }
        public string[] Indices { get; }

        protected override List<Command> CurrentCommands { get => new List<Command>(); }

        public Store(params string[] schemaDefinitions)
        {
            if (schemaDefinitions.Length == 0)
            {
                throw new ArgumentException("Must contain at least one element", nameof(schemaDefinitions));
            }

            SchemaDefinitions = schemaDefinitions.Select(Camelizer.ToCamelCase).ToArray();

            Indices = schemaDefinitions.Select(d => d
            .TrimStart('+')
            .TrimStart('&')
            .TrimStart('*'))
                .ToArray();

            PrimaryKey = Indices.First();   
        }

        public async Task<TKey> Add(T item, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("add", cancellationToken, item);
        }

        public async Task<TKey> Add(T item, TKey? key, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("add", cancellationToken, item, key);
        }

        public async Task<TKey> AddBlob(byte[] data, TKey? key, string mimeType = "",CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("addBlob", cancellationToken, data, key, mimeType);
        }

        public async Task<TKey> AddObjectUrl(string objectUrl, TKey? key,CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("addObjectUrl", cancellationToken, objectUrl, key);
        }

        public async Task<TKey> BulkAdd(IEnumerable<T> items,CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("bulkAdd", cancellationToken, items);
        }

        public async Task BulkAdd(IEnumerable<T> items, IEnumerable<TKey>? keys, CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery("bulkAdd", cancellationToken, items, keys);
        }

        public async Task<List<TKey>> BulkAddReturnAllKeys(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            return await Execute<List<TKey>>("bulkAdd", cancellationToken, items, new { allKeys = true });
        }

        public async Task BulkDelete(IEnumerable<TKey> primaryKeys, CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery("bulkDelete", cancellationToken, primaryKeys);
        }

        public async Task<T?[]> BulkGet(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        {
            return await Execute<T[]>("bulkGet", cancellationToken, keys);
        }

        public async Task<TKey> BulkPut(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("bulkPut", cancellationToken, items);
        }

        public async Task BulkPut(IEnumerable<T> items, IEnumerable<TKey>? keys, CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery("bulkPut", cancellationToken, items, keys);
        }

        public async Task<List<TKey>> BulkPutReturnAllKeys(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            return await Execute<List<TKey>>("bulkPut", cancellationToken, items, new { allKeys = true });
        }

        public async Task Delete(TKey primaryKey, CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery("delete", cancellationToken, primaryKey);
        }

        public async Task<T?> Get(TKey primaryKey, CancellationToken cancellationToken = default)
        {
            return await Execute<T?>("get", cancellationToken, primaryKey);
        }

        public async Task<byte[]> GetBlob(TKey primaryKey, CancellationToken cancellationToken = default)
        {
            if (CommandExecuterJsInterop.RunInBrowser)
            {

                return await Execute<byte[]>("getBlob", cancellationToken, primaryKey);
            }
            else
            {
                var base64 = await Execute<string>("getBlob", cancellationToken, primaryKey);
                return Convert.FromBase64String(base64);
            }
        }

        public async Task<string> GetObjectUrl(TKey primaryKey, CancellationToken cancellationToken = default)
        {
            return await Execute<string>("getObjectUrl", cancellationToken, primaryKey);
        }

        public Collection<T, TKey> OrderBy(string index)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("orderBy", Camelizer.ToCamelCase(index));
            return collection;
        }

        public async Task<TKey> Put(T item, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("put", cancellationToken, item);
        }

        public async Task<TKey> Put(T item, TKey? key, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("put", cancellationToken, item, key);
        }

        public async Task<TKey> PutBlob(byte[] data, TKey? key, string mimeType = "", CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("putBlob", cancellationToken, data, key, mimeType);
        }

        public async Task<TKey> PutObjectUrl(string objectUrl, TKey? key, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>("putObjectUrl", cancellationToken, objectUrl, key);
        }

        public async Task<int> Update(TKey key, Dictionary<string, object> changes, CancellationToken cancellationToken = default)
        {
            var changesObject = new ExpandoObject();
            foreach (var change in changes)
            {
                changesObject.TryAdd(Camelizer.ToCamelCase(change.Key), change.Value);
            }

            return await Execute<int>("update", cancellationToken, key, changesObject);
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

        public async Task Clear(CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery("clear", cancellationToken);
        }

        public bool HasIndex(string index)
        {
            return Indices.Contains(index); 
        }

        protected override Collection<T, TKey> CreateNewColletion()
        {
            return new Collection<T, TKey>(Db, StoreName, CommandExecuterJsInterop);
        }
    }
}
