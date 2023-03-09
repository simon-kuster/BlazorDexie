using BlazorDexie.JsModule;

namespace BlazorDexie.JsInterop
{
    public sealed class StaticCommandExecuterJsInterop : IAsyncDisposable
    {
        private IModule? _module;

        public StaticCommandExecuterJsInterop(IModuleFactory jsModuleFactory)
        {
            _module = jsModuleFactory.CreateModule("scripts/staticCommandExecuter.js");
        }

        public async Task<T> Execute<T>(Command command, CancellationToken cancellationToken)
        {
            return await GetModule().InvokeAsync<T>("execute", cancellationToken, command);
        }

        public async Task ExecuteNonQuery(Command command, CancellationToken cancellationToken)
        {
            await GetModule().InvokeVoidAsync("executeNonQuery", cancellationToken, command);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.DisposeAsync().ConfigureAwait(false);
                _module = null;
            }
        }

        private IModule GetModule()
        {
            if (_module == null)
            {
                throw new ObjectDisposedException("_module is disposed");
            }

            return _module;
        }
    }
}
