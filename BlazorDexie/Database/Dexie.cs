using BlazorDexie.JsInterop;
using BlazorDexie.Options;
using Microsoft.Extensions.Logging;

namespace BlazorDexie.Database
{
    public sealed class Dexie
    {
        private readonly StaticCommandExecuterJsInterop _staticCommandExecuterJsInterop;
        private ILogger _logger;

        public Dexie(BlazorDexieOptions blazorDexieOptions)
        {
            _staticCommandExecuterJsInterop = new StaticCommandExecuterJsInterop(blazorDexieOptions.ModuleFactory);
            _logger = blazorDexieOptions.LoggerFactory.CreateLogger<Dexie>();
        }

        public async Task Delete(string databaseName, CancellationToken cancellationToken = default)
        {
            await _staticCommandExecuterJsInterop.ExecuteNonQuery(new Command("delete", databaseName), cancellationToken);
            _logger.LogInformation($"Dexie.delete({databaseName})");
        }

        public async Task<bool> Exits(string databaseName, CancellationToken cancellationToken = default)
        {
            var returnValue = await _staticCommandExecuterJsInterop.Execute<bool>(new Command("exists", databaseName), cancellationToken);

            _logger.LogInformation($"Dexie.exists({databaseName})");
            return returnValue;
        }

        public async ValueTask DisposeAsync()
        {
            await _staticCommandExecuterJsInterop.DisposeAsync().ConfigureAwait(false);
        }
    }
}
