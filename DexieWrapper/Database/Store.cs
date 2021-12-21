using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public class Store<T> : Collection<T>, IStore
    {
        public string[] Indices { get; }

        public Store(params string[] indices)
        {
            Indices = indices;
        }

        public async Task<T?> Get(object primaryKey)
        {
            MainCommand command = new MainCommand(_storeName, "get", new List<object?> { primaryKey });
            return await _commandExecuterJsInterop.Execute<T?>(_dbDefinition, command);
        }

        public async Task<T?[]?> BulkGet(object[] primaryKeys)
        {
            List<T?> result = new List<T?>();

            foreach (var key in primaryKeys)
            {
                result.Add(await Get(key));
            }

            return result.ToArray();
        }

        public async Task Put(T item, object? key = null)
        {
            MainCommand command = new MainCommand(_storeName, "put", new List<object?> { item, key });
            await _commandExecuterJsInterop.ExecuteNonQuery(_dbDefinition, command);
        }

        public async Task BulkPut(T[] items, object?[]? keys = null)
        {
            if (keys == null)
                keys = new object?[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                await Put(items[i], keys[i]);
            }
        }

        public async Task Delete(object primaryKey)
        {
            MainCommand command = new MainCommand(_storeName, "delete", new List<object?> { primaryKey });
            await _commandExecuterJsInterop.ExecuteNonQuery(_dbDefinition, command);
        }

        public async Task BulkDelete(object[] primaryKeys)
        {
            foreach (var key in primaryKeys)
            {
                await Delete(key);
            }
        }

        public WhereClause<T> Where(string keyPathArray)
        {
            var collection = CreateNewColletion();
            collection.CurrentCommand = new MainCommand(_storeName, "where", new List<object?> { keyPathArray });
            return new WhereClause<T>(collection);
        }

        private Collection<T> CreateNewColletion()
        {
            return new Collection<T>(_dbDefinition, _storeName, _commandExecuterJsInterop);
        }
    }
}
