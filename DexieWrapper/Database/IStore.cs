using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public interface IStore
    {
        public string[] Indices { get; }

        public void Init(Db db, string storeName, CommandExecuterJsInterop commandExecuterJsInterop);
    }
}
