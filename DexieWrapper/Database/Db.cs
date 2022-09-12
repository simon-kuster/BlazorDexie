using Microsoft.JSInterop;
using Nosthy.Blazor.DexieWrapper.Definitions;
using Nosthy.Blazor.DexieWrapper.JsInterop;
using Nosthy.Blazor.DexieWrapper.JsModule;
using System.Text.Json.Serialization;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public abstract class Db : IDisposable, IAsyncDisposable
    {
        private readonly CommandExecuterJsInterop _commandExecuterJsInterop;
        private string? _dbJsReferenceDatabaseName;
        private bool disposed = false;

        public string DefaultDatabaseName { get; }
        public int VersionNumber { get; }
        public List<DbVersionDefinition> Versions { get; } = new List<DbVersionDefinition>();
        public IJSObjectReference? DbJsReference { get; private set; }

        [JsonIgnore]
        public IEnumerable<DbVersion> PreviousVersions { get; }

        public Db(string defaultDatabaseName, int currentVersionNumber, IEnumerable<DbVersion> previousVersions, IModuleFactory jsModuleFactory)
        {
            DefaultDatabaseName = defaultDatabaseName;
            VersionNumber = currentVersionNumber;
            PreviousVersions = previousVersions;
            _commandExecuterJsInterop = new CommandExecuterJsInterop(jsModuleFactory);

            var latestVersion = new DbVersionDefinition(VersionNumber);

            var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                if (typeof(IStore).IsAssignableFrom(property.PropertyType))
                {
                    var store = (IStore?)property.GetValue(this);

                    if (store != null)
                    {
                        var storeName = property.Name;
                        store.Init(this, storeName, _commandExecuterJsInterop);
                        latestVersion.Stores.Add(new StoreDefinition(storeName, store.SchemaDefinitions));
                    }
                }
            }

            Versions.Add(latestVersion);

            foreach (var version in PreviousVersions)
            {
                Versions.Add(version.GetDefinition());
            }
        }

        public async Task Init(string databaseName, CancellationToken cancellationToken)
        {
            if ((DbJsReference == null || _dbJsReferenceDatabaseName != databaseName) && _commandExecuterJsInterop.CanUseObjectReference)
            {
                if (DbJsReference != null)
                {
                    await DbJsReference.DisposeAsync();
                }

                // Optimized code for Blazor
                // Create Dexie object only once
                DbJsReference = await _commandExecuterJsInterop.InitDb(databaseName, Versions, cancellationToken);
                _dbJsReferenceDatabaseName = databaseName;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _commandExecuterJsInterop.Dispose();
                }
            }
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (DbJsReference != null)
            {
                await DbJsReference.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
