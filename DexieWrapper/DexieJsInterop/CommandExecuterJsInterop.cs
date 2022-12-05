using Microsoft.JSInterop;
using Nosthy.Blazor.DexieWrapper.Definitions;
using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.DexieJsInterop
{
    public class CommandExecuterJsInterop : IDisposable
    {
        private readonly IModule _module;
        private bool disposed = false;

        public bool RunInBrowser { get; set; }

        public CommandExecuterJsInterop(IModuleFactory jsModuleFactory)
        {
            _module = jsModuleFactory.CreateModule("scripts/commandExecuter.js");
            RunInBrowser = _module is EsModule;
        }

        public async Task<T> InitDbAndExecute<T>(string databaseName, List<DbVersionDefinition> versions, string storeName, List<Command> commands, 
            CancellationToken cancellationToken)
        {
            return await _module.InvokeAsync<T>("initDbAndExecute", cancellationToken, databaseName, versions, storeName, commands);
        }

        public async Task<T> Execute<T>(IJSObjectReference dbJsObjectRef, string storeName, List<Command> commands, CancellationToken cancellationToken)
        {
            return await _module.InvokeAsync<T>("execute", cancellationToken, dbJsObjectRef, storeName, commands);
        }

        public async Task InitDbAndExecuteNonQuery(string databaseName, List<DbVersionDefinition> versions, string storeName, List<Command> commands, 
            CancellationToken cancellationToken)
        {
            await _module.InvokeVoidAsync("initDbAndExecuteNonQuery", cancellationToken, databaseName, versions, storeName, commands);
        }

        public async Task ExecuteNonQuery(IJSObjectReference dbJsObjectRef, string storeName, List<Command> commands, CancellationToken cancellationToken)
        {
            await _module.InvokeVoidAsync("executeNonQuery", cancellationToken, dbJsObjectRef, storeName, commands);
        }

        public async Task<IJSObjectReference> InitDb(string databaseName, List<DbVersionDefinition> versions, CancellationToken cancellationToken)
        {
            return await _module.InvokeAsync<IJSObjectReference>("initDb", cancellationToken, databaseName, versions);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _module.Dispose();
                }
            }
        }
    }
}
