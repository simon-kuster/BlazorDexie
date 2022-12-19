using Microsoft.JSInterop;
using Nosthy.Blazor.DexieWrapper.Definitions;
using Nosthy.Blazor.DexieWrapper.JsInterop;
using Nosthy.Blazor.DexieWrapper.JsModule;
using System.Text.Json.Serialization;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public abstract class Db : IAsyncDisposable
    {
        private readonly CollectionCommandExecuterJsInterop _collectionCommandExecuterJsInterop;
        private readonly StaticCommandExecuterJsInterop _staticCommandExecuterJsInterop;
        private string? _dbJsReferenceDatabaseName;

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
            _collectionCommandExecuterJsInterop = new CollectionCommandExecuterJsInterop(jsModuleFactory);
            _staticCommandExecuterJsInterop = new StaticCommandExecuterJsInterop(jsModuleFactory);

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
                        store.Init(this, storeName, _collectionCommandExecuterJsInterop);
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
            if ((DbJsReference == null || _dbJsReferenceDatabaseName != databaseName) && _collectionCommandExecuterJsInterop.CanUseObjectReference)
            {
                if (DbJsReference != null)
                {
                    await DbJsReference.DisposeAsync();
                }

                // Optimized code for Blazor
                // Create Dexie object only once
                DbJsReference = await _collectionCommandExecuterJsInterop.InitDb(databaseName, Versions, cancellationToken);
                _dbJsReferenceDatabaseName = databaseName;
            }
        }

        public async Task Delete(CancellationToken cancellationToken = default)
        {
            await _staticCommandExecuterJsInterop.ExecuteNonQuery(new Command("delete", DefaultDatabaseName), cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            await _collectionCommandExecuterJsInterop.DisposeAsync().ConfigureAwait(false);
            await _staticCommandExecuterJsInterop.DisposeAsync().ConfigureAwait(false);

            if (DbJsReference != null)
            {
                await DbJsReference.DisposeAsync().ConfigureAwait(false);
                DbJsReference = null;
            }
        }
    }
}
