using DexieWrapper.Definitions;
using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public class Store<T> : IStore
    {
        private string _storeName = null!;
        private DbDefinition _dbDefinition = null!;
        private CommandExecuterJsInterop _commandExecuterJsInterop = null!;

        public string[] Indices { get; }

        public Store(params string[] indices)
        {
            Indices = indices;
        }

        public void Init(DbDefinition dbDefinition, string storeName, CommandExecuterJsInterop commandExecuterJsInterop)
        {
            _storeName = storeName;
            _dbDefinition = dbDefinition;
            _commandExecuterJsInterop = commandExecuterJsInterop;
        }

        public async Task<T?> Get(object primaryKey)
        {
            Command command = new Command(_storeName, "get", new List<object?> { primaryKey });
            return await _commandExecuterJsInterop.Execute<T?>(_dbDefinition, command);
        }

        public async Task Put(T item, object? key = null)
        {
            Command command = new Command(_storeName, "put", new List<object?> { item, key });
            await _commandExecuterJsInterop.ExecuteNonQuery(_dbDefinition, command);
        }
    }
}
