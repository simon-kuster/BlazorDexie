using DexieWrapper.Definitions;
using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public interface IStore
    {
        public string[] Indices { get; }

        public void Init(DbDefinition dbDefinition, string storeName, CommandExecuterJsInterop commandExecuterJsInterop);
    }
}
