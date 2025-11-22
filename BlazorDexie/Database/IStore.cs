using BlazorDexie.Database.Transaction;
using BlazorDexie.JsInterop;
using Microsoft.Extensions.Logging;

namespace BlazorDexie.Database
{
    public interface IStore
    {
        string[] SchemaDefinitions { get; }
        void Init(IDb db, string storeName, CollectionCommandExecuterJsInterop commandExecuterJsInterop, ILogger logger);
        void SetTransactionWrapper(TransactionBodyWrapper transactionBodyWrapper);
        void ClearTransactionWrapper();
    }
}
