using Microsoft.JSInterop;
using Nosthy.Blazor.DexieWrapper.Definitions;
using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.JsInterop
{
    public sealed class CollectionCommandExecuterJsInterop : IAsyncDisposable
    {
        private IModule? _module;

        public bool CanUseObjectReference { get; set; }

        public CollectionCommandExecuterJsInterop(IModuleFactory jsModuleFactory)
        {
            _module = jsModuleFactory.CreateModule("scripts/collectionCommandExecuter.js");
            CanUseObjectReference = _module is EsModule;
        }

        public async Task<T> InitDbAndExecute<T>(string databaseName, List<DbVersionDefinition> versions, string storeName, List<Command> commands, 
            CancellationToken cancellationToken)
        {
            return await GetModule().InvokeAsync<T>("initDbAndExecute", cancellationToken, databaseName, versions, storeName, commands);
        }

        public async Task<T> Execute<T>(IJSObjectReference dbJsObjectRef, string storeName, List<Command> commands, CancellationToken cancellationToken)
        {
            return await GetModule().InvokeAsync<T>("execute", cancellationToken, dbJsObjectRef, storeName, commands);
        }

        public async Task InitDbAndExecuteNonQuery(string databaseName, List<DbVersionDefinition> versions, string storeName, List<Command> commands, 
            CancellationToken cancellationToken)
        {
            await GetModule().InvokeVoidAsync("initDbAndExecuteNonQuery", cancellationToken, databaseName, versions, storeName, commands);
        }

        public async Task ExecuteNonQuery(IJSObjectReference dbJsObjectRef, string storeName, List<Command> commands, CancellationToken cancellationToken)
        {
            await GetModule().InvokeVoidAsync("executeNonQuery", cancellationToken, dbJsObjectRef, storeName, commands);
        }

        public async Task<IJSObjectReference> InitDb(string databaseName, List<DbVersionDefinition> versions, CancellationToken cancellationToken)
        {
            return await GetModule().InvokeAsync<IJSObjectReference>("initDb", cancellationToken, databaseName, versions);
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
