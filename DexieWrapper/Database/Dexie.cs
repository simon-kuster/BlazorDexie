using Nosthy.Blazor.DexieWrapper.JsInterop;
using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public sealed class Dexie
    {
        private readonly StaticCommandExecuterJsInterop _staticCommandExecuterJsInterop;

        public Dexie(IModuleFactory jsModuleFactory)
        {
            _staticCommandExecuterJsInterop = new StaticCommandExecuterJsInterop(jsModuleFactory);
        }

        public async Task Delete(string databaseName, CancellationToken cancellationToken = default)
        {
            await _staticCommandExecuterJsInterop.ExecuteNonQuery(new Command("delete", databaseName), cancellationToken);
        }

        public async Task<bool> Exits(string databaseName, CancellationToken cancellationToken = default)
        {
            return await _staticCommandExecuterJsInterop.Execute<bool>(new Command("exists", databaseName), cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _staticCommandExecuterJsInterop.DisposeAsync().ConfigureAwait(false);
        }
    }
}
