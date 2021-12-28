using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public class Store<T> : Collection<T>, IStore
    {
        public string[] Indices { get; }

        protected override List<Command> CurrentCommands { get => new List<Command>(); }

        public Store(params string[] indices)
        {
            Indices = indices;
        }

        public async Task<T?> Get(object primaryKey)
        {
            return await Execute<T?>("get", primaryKey);
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
            await ExecuteNonQuery("put", item, key);
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
            await Execute<T>("delete", primaryKey);
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
            collection.AddCommand("where", keyPathArray);
            return new WhereClause<T>(collection);
        }

        private Collection<T> CreateNewColletion()
        {
            return new Collection<T>(Db, StoreName, CommandExecuterJsInterop);
        }
    }
}
