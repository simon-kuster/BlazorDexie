using Nosthy.Blazor.DexieWrapper.DexieJsInterop;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public interface IStore
    {
        public string[] SchemaDefinitions { get; }

        public void Init(Db db, string storeName, CommandExecuterJsInterop commandExecuterJsInterop);
    }
}
