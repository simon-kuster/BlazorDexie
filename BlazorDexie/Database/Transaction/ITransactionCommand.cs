using BlazorDexie.JsInterop;

namespace BlazorDexie.Database.Transaction
{
    public interface ITransactionCommand
    {
        string Id { get; } 
        TransactionCommandType Type { get; }
        string StoreName { get; }
        List<Command> Commands { get; } 
    }
}
