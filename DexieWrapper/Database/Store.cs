using Nosthy.Blazor.DexieWrapper.Blob;
using Nosthy.Blazor.DexieWrapper.DexieJsInterop;
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

        public async Task<TKey> Add(T item, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>(
                "add", 
                new object?[] { item }, 
                databaseName ?? Db.DefaultDatabaseName, 
                new CommandParameters { BlobDataConvert = BlobDataConvertFactory.CreatetForWrite(0, false) }, 
                cancellationToken);
        }

        public async Task<TKey> Add(T item, TKey? key, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>(
                "add", 
                new object?[] { item, key },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters() { BlobDataConvert = BlobDataConvertFactory.CreatetForWrite(0, false) },
                cancellationToken);
        }

        public async Task<TKey> BulkAdd(IEnumerable<T> items, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>(
                "bulkAdd",
                new object?[] { items },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters { BlobDataConvert = BlobDataConvertFactory.CreatetForWrite(0, true) },
                cancellationToken);
        }

        public async Task BulkAdd(IEnumerable<T> items, IEnumerable<TKey>? keys, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery(
                "bulkAdd", 
                new object?[] { items, keys },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters{ BlobDataConvert = BlobDataConvertFactory.CreatetForWrite(0, true) }, 
                cancellationToken);
        }

        public async Task<List<TKey>> BulkAddReturnAllKeys(IEnumerable<T> items, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            return await Execute<List<TKey>>(
                "bulkAdd",
                new object?[] { items, new { allKeys = true } },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters { BlobDataConvert = BlobDataConvertFactory.CreatetForWrite(0, true) },
                cancellationToken);
        }

        public async Task BulkDelete(IEnumerable<TKey> primaryKeys, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery(
                "bulkDelete", 
                new object?[] { primaryKeys }, 
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters(), 
                cancellationToken);
        }


        public async Task<T?[]> BulkGet(IEnumerable<TKey> keys, string? databaseName = null, BlobDataFormat blobDataFormat = BlobDataFormat.ByteArray, 
            CancellationToken cancellationToken = default)
        {
            return await Execute<T[]>(
                "bulkGet",
                new object?[] { keys },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters { BlobDataConvert = BlobDataConvertFactory.CreateForRead(true, blobDataFormat) },
                cancellationToken);
        }

        public async Task<TKey> BulkPut(IEnumerable<T> items, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>(
                "bulkPut",
                new object?[] { items },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters { BlobDataConvert = BlobDataConvertFactory.CreatetForWrite(0, true) },
                cancellationToken);
        }

        public async Task BulkPut(IEnumerable<T> items, IEnumerable<TKey>? keys, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery(
                "bulkPut", 
                new object?[] { items, keys }, 
                databaseName ?? Db.DefaultDatabaseName, 
                new CommandParameters(), 
                cancellationToken);
        }

        public async Task<List<TKey>> BulkPutReturnAllKeys(IEnumerable<T> items, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            return await Execute<List<TKey>>(
                "bulkPut", 
                new object?[] { items, new { allKeys = true } },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters{ BlobDataConvert = BlobDataConvertFactory.CreatetForWrite(0, true) }, 
                cancellationToken);
        }

        public async Task Delete(TKey primaryKey, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery(
                "delete", 
                new object?[] { primaryKey },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters(), cancellationToken);
        }

        public async Task<T?> Get(TKey primaryKey, string? databaseName = null, BlobDataFormat blobDataFormat = BlobDataFormat.ObjectUrl, CancellationToken cancellationToken = default)
        {
            return await Execute<T?>(
                "get", 
                new object?[] { primaryKey }, 
                databaseName ?? Db.DefaultDatabaseName, 
                new CommandParameters { BlobDataConvert = BlobDataConvertFactory.CreateForRead(false, blobDataFormat) }, 
                cancellationToken);
        }

        public Collection<T, TKey> OrderBy(string index)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("orderBy", Camelizer.ToCamelCase(index));
            return collection;
        }

        public async Task<TKey> Put(T item, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>(
                "put",
                new object?[] { item },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters { BlobDataConvert = BlobDataConvertFactory.CreatetForWrite(0, false) },
                cancellationToken);
        }

        public async Task<TKey> Put(T item, TKey? key, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            return await Execute<TKey>(
                "put", 
                new object?[] { item, key },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters { BlobDataConvert = BlobDataConvertFactory.CreatetForWrite(0, false) },
                cancellationToken);
        }

        public async Task<int> Update(TKey key, Dictionary<string, object> changes, string? databaseName = null, CancellationToken cancellationToken = default)
        {
            var changesObject = new ExpandoObject();
            foreach (var change in changes)
            {
                changesObject.TryAdd(Camelizer.ToCamelCase(change.Key), change.Value);
            }

            // Todo add support for BlobDataConvert
            return await Execute<int>(
                "update", 
                new object?[] { key, changesObject },
                databaseName ?? Db.DefaultDatabaseName,
                new CommandParameters(),
                cancellationToken);
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

        public async Task Clear(string? databaseName = null, CancellationToken cancellationToken = default)
        {
            await ExecuteNonQuery(
                "clear", 
                new object?[] { }, 
                databaseName ?? Db.DefaultDatabaseName, 
                new CommandParameters(), 
                cancellationToken);
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
