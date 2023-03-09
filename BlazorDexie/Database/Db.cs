using BlazorDexie.Definitions;
using BlazorDexie.JsInterop;
using BlazorDexie.JsModule;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace BlazorDexie.Database
{
    public abstract class Db : IAsyncDisposable
    {
        private readonly CollectionCommandExecuterJsInterop _collectionCommandExecuterJsInterop;
        private readonly StaticCommandExecuterJsInterop _staticCommandExecuterJsInterop;

        public string DatabaseName { get; }
        public int VersionNumber { get; }
        public List<DbVersionDefinition> Versions { get; } = new List<DbVersionDefinition>();
        public IJSObjectReference? DbJsReference { get; private set; }

        [JsonIgnore]
        public IEnumerable<DbVersion> PreviousVersions { get; }

        public Db(string databaseName, int currentVersionNumber, IEnumerable<DbVersion> previousVersions, IModuleFactory jsModuleFactory)
        {
            DatabaseName = databaseName;
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

        public async Task Init(CancellationToken cancellationToken)
        {
            if (DbJsReference == null && _collectionCommandExecuterJsInterop.RunInBrowser)
            {
                // Optimized code for Blazor
                // Create Dexie object only once
                DbJsReference = await _collectionCommandExecuterJsInterop.InitDb(DatabaseName, Versions, cancellationToken);
            }
        }

        public async Task Delete(CancellationToken cancellationToken = default)
        {
            await _staticCommandExecuterJsInterop.ExecuteNonQuery(new Command("delete", DatabaseName), cancellationToken);
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
