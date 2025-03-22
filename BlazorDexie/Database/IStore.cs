using BlazorDexie.JsInterop;
using Microsoft.Extensions.Logging;

namespace BlazorDexie.Database
{
    public interface IStore
    {
        public string[] SchemaDefinitions { get; }

        public void Init(IDb db, string storeName, CollectionCommandExecuterJsInterop commandExecuterJsInterop, ILogger logger);
    }
}
