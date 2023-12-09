using Microsoft.JSInterop;

namespace BlazorDexie.Database
{
    public class TransactionBodyWrapper
    {
        private readonly Func<Task> _transactionBody;

        public TransactionBodyWrapper(Func<Task> transactionBody)
        {
            _transactionBody = transactionBody;
        }

        [JSInvokable]
        public async Task CallTransactionBody()
        {
            await _transactionBody();
        }
    }
}
