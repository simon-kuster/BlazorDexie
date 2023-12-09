using BlazorDexie.Definitions;
using BlazorDexie.JsInterop;
using BlazorDexie.JsModule;
using BlazorDexie.Utils;
using Microsoft.JSInterop;

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

        public Db(
            string databaseName, 
            int currentVersionNumber, 
            IEnumerable<DbVersion> previousVersions, 
            IModuleFactory jsModuleFactory,
            string? upgrade = null, 
            string? upgradeModule = null,
            bool camelCaseStoreNames = false)
        {
            DatabaseName = databaseName;
            VersionNumber = currentVersionNumber;
            _collectionCommandExecuterJsInterop = new CollectionCommandExecuterJsInterop(jsModuleFactory);
            _staticCommandExecuterJsInterop = new StaticCommandExecuterJsInterop(jsModuleFactory);

            var latestVersion = new DbVersionDefinition(VersionNumber, upgrade, upgradeModule);

            var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                if (typeof(IStore).IsAssignableFrom(property.PropertyType))
                {
                    var store = (IStore?)property.GetValue(this);

                    if (store != null)
                    {
                        var storeName = camelCaseStoreNames ? Camelizer.ToCamelCase(property.Name) : property.Name;
                        store.Init(this, storeName, _collectionCommandExecuterJsInterop);
                        latestVersion.Stores.Add(new StoreDefinition(storeName, store.SchemaDefinitions));
                    }
                }
            }

            var versions = new List<DbVersionDefinition>() { latestVersion };
            
            versions.AddRange(previousVersions
                    .Select(v => v.GetDefinition(camelCaseStoreNames))
                    .ToList());

            Versions = versions
                .OrderByDescending(v => v.VersionNumber)
                .ToList();
        }

        public async Task Init(CancellationToken cancellationToken)
        {
            if (DbJsReference == null && _collectionCommandExecuterJsInterop.RunInBrowser)
            {
                // Optimized code for Blazor
                // Create Dexie object only once
                DbJsReference = await _collectionCommandExecuterJsInterop.InitDb(DatabaseName, Versions, cancellationToken);
            }

            await _collectionCommandExecuterJsInterop.SetUserModuleBasePath(cancellationToken);
        }

        public async Task Delete(CancellationToken cancellationToken = default)
        {
            await _staticCommandExecuterJsInterop.ExecuteNonQuery(new Command("delete", DatabaseName), cancellationToken);
        }

        public async Task Transaction(
            string mode, 
            string[] storeNames, 
            int timeout, 
            Func<Task> body,
            Func<Task> complete, 
            Func<string, Task> failed,
            CancellationToken cancellationToken = default)
        {
            var camelizedStoreNames = storeNames.Select(Camelizer.ToCamelCase).ToArray();
            var transactionHandlers = new TransactionHandlers(body, complete, failed);
            var command = new Command("transaction", DbJsReference, mode, camelizedStoreNames, timeout, DotNetObjectReference.Create(transactionHandlers));
            await _staticCommandExecuterJsInterop.ExecuteNonQuery(command, cancellationToken);
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
