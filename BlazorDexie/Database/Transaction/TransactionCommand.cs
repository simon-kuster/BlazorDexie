using BlazorDexie.JsInterop;

namespace BlazorDexie.Database.Transaction
{
    public class TransactionCommand(TransactionCommandType type, string storeName, List<Command> commands): ITransactionCommand
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public TaskCompletionSource<object?> Tcs { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TransactionCommandType Type { get; } = type;
        public string StoreName { get; } = storeName;
        public List<Command> Commands { get; } = commands;
    }
}
