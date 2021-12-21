using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public class Store<T> : Collection<T>, IStore
    {
        public string[] Indices { get; }

        public override List<Command> CurrentCommands { get => new List<Command>(); }

        public Store(params string[] indices)
        {
            Indices = indices;
        }

        public async Task<T?> Get(object primaryKey)
        {
            return await Execute<T?>(new Command("get", new List<object?> { primaryKey }));
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
            await ExecuteNonQuery(new Command("put", new List<object?> { item, key }));
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
            await Execute<T>(new Command("delete", new List<object?> { primaryKey }));
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
            collection.AddCommand(new Command("where", new List<object?> { keyPathArray }));
            return new WhereClause<T>(collection);
        }

        private Collection<T> CreateNewColletion()
        {
            return new Collection<T>(_dbDefinition, _storeName, _commandExecuterJsInterop);
        }
    }
}
