using BlazorDexie.Definitions;
using BlazorDexie.JsInterop;
using BlazorDexie.Options;
using BlazorDexie.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Reflection;

namespace BlazorDexie.Database
{
    public abstract class Db<TConcrete> : IAsyncDisposable, IDb
    {
        private static PropertyInfo[] Properties;
        private static Dictionary<string, Func<object, object?>> PropertyGetterDictionary;

        private readonly CollectionCommandExecuterJsInterop _collectionCommandExecuterJsInterop;
        private readonly StaticCommandExecuterJsInterop _staticCommandExecuterJsInterop;
        private readonly BlazorDexieOptions _blazorDexieOptions;
        private readonly ILogger _logger;

        public string DatabaseName { get; }
        public int VersionNumber { get; }
        public List<DbVersionDefinition> Versions { get; internal protected set; } = new List<DbVersionDefinition>();
        public IJSObjectReference? DbJsReference { get; private set; }

        protected Db(
            string databaseName,
            int currentVersionNumber,
            IEnumerable<IDbVersion> previousVersions,
            BlazorDexieOptions blazorDexieOptions,
            string? upgrade = null,
            string? upgradeModule = null)
        {
            DatabaseName = databaseName;
            VersionNumber = currentVersionNumber;
            _blazorDexieOptions = blazorDexieOptions;
            _collectionCommandExecuterJsInterop = new CollectionCommandExecuterJsInterop(blazorDexieOptions.ModuleFactory);
            _staticCommandExecuterJsInterop = new StaticCommandExecuterJsInterop(blazorDexieOptions.ModuleFactory);
            _logger = blazorDexieOptions.LoggerFactory.CreateLogger("BlazorDexie.Database.Db");

            Versions = InitStoresAndGetVersionDefinitions(previousVersions, upgrade, upgradeModule);
        }

        static Db()
        {
            Properties = typeof(TConcrete).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => typeof(IStore).IsAssignableFrom(p.PropertyType))
                .ToArray();

            PropertyGetterDictionary = Properties.ToDictionary(p => p.Name, p => PropertyAccessorDelegateBuilder.BuildPropertyGetter(p));
        }

        private List<DbVersionDefinition> InitStoresAndGetVersionDefinitions(
            IEnumerable<IDbVersion> previousVersions,
            string? upgrade = null,
            string? upgradeModule = null)
        {
            var latestVersion = new DbVersionDefinition(VersionNumber, upgrade, upgradeModule);

            foreach (var property in Properties)
            {
                var store = (IStore?)PropertyGetterDictionary[property.Name](this);
                if (store != null)
                {
                    var storeName = _blazorDexieOptions.CamelCaseStoreNames ? Camelizer.ToCamelCase(property.Name) : property.Name;
                    store.Init(this, storeName, _collectionCommandExecuterJsInterop, _blazorDexieOptions.LoggerFactory.CreateLogger("BlazorDexie.Database.Collection"));
                    latestVersion.Stores.Add(new StoreDefinition(storeName, store.SchemaDefinitions));
                }                
            }

            var versions = new List<DbVersionDefinition>() { latestVersion };

            versions.AddRange(previousVersions
                    .Select(v => v.GetDefinition(_blazorDexieOptions.CamelCaseStoreNames))
                    .ToList());

            return versions
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

        public async Task Close(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (DbJsReference != null)
            {
                await _collectionCommandExecuterJsInterop.Close(DbJsReference);
            }
        }

        public async Task Delete(CancellationToken cancellationToken = default)
        {
            await _staticCommandExecuterJsInterop.ExecuteNonQuery(new Command("delete", DatabaseName), cancellationToken);
            _logger.LogInformation($"Dexie.delete({DatabaseName})");
        }

        public async Task Transaction(string mode, string[] storeNames, int timeout, Func<Task> transactionBody, CancellationToken cancellationToken = default)
        {
            var transformedStoreNames = _blazorDexieOptions.CamelCaseStoreNames ? storeNames.Select(Camelizer.ToCamelCase).ToArray() : storeNames;
            var transactionBodyWrapper = new TransactionBodyWrapper(transactionBody);
            var command = new Command("transaction", DbJsReference, mode, transformedStoreNames, timeout, DotNetObjectReference.Create(transactionBodyWrapper));

            _logger.LogInformation("Beginn transaction");
            await _staticCommandExecuterJsInterop.ExecuteNonQuery(command, cancellationToken);
            _logger.LogInformation("End transaction");
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (DbJsReference != null)
            {
                await Close();
                await DbJsReference.DisposeAsync().ConfigureAwait(false);
                DbJsReference = null;
            }

            await _collectionCommandExecuterJsInterop.DisposeAsync().ConfigureAwait(false);
            await _staticCommandExecuterJsInterop.DisposeAsync().ConfigureAwait(false);
        }
    }
}
