using Microsoft.JSInterop;

namespace BlazorDexie.Database
{
    public class TransactionHandlers
    {
        private readonly Func<Task> _body;
        private readonly Func<Task> _complete;
        private readonly Func<string, Task> _failed;

        public TransactionHandlers(Func<Task> body, Func<Task> complete, Func<string, Task> failed)
        {
            _body = body;
            _complete = complete;
            _failed = failed;
        }

        [JSInvokable]
        public async Task CallBody()
        {
            await _body();
        }

        [JSInvokable]
        public async Task CallComplete()
        {
            await _complete();
        }

        [JSInvokable]
        public async Task CallFailed(string errorMessage)
        {
            await _failed(errorMessage);
        }
    }
}
