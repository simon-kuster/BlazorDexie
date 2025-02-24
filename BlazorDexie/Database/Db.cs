using BlazorDexie.Definitions;
using BlazorDexie.JsInterop;
using BlazorDexie.JsModule;
using BlazorDexie.Utils;
using Microsoft.JSInterop;
using System.Reflection;

namespace BlazorDexie.Database
{
    public abstract class Db : IAsyncDisposable
    {
        private readonly CollectionCommandExecuterJsInterop _collectionCommandExecuterJsInterop;
        private readonly StaticCommandExecuterJsInterop _staticCommandExecuterJsInterop;
        private readonly bool _camelCaseStoreNames;

        public string DatabaseName { get; }
        public int VersionNumber { get; }
        public List<DbVersionDefinition> Versions { get; internal protected set; } = new List<DbVersionDefinition>();
        public IJSObjectReference? DbJsReference { get; private set; }

        [Obsolete("Use Db<TConcrete> instead for better performance")]
        public Db(
            string databaseName,
            int currentVersionNumber,
            IEnumerable<DbVersion> previousVersions,
            IModuleFactory moduleFactory,
            string? upgrade = null,
            string? upgradeModule = null,
            bool camelCaseStoreNames = false) : this(databaseName, currentVersionNumber, moduleFactory, camelCaseStoreNames)
        {
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Versions = InitStoresAndGetVersionDefinitions(previousVersions, properties, upgrade, upgradeModule);
        }

        internal protected Db(
            string databaseName,
            int currentVersionNumber,
            IModuleFactory moduleFactory,
            bool camelCaseStoreNames = false)
        {
            DatabaseName = databaseName;
            VersionNumber = currentVersionNumber;
            _camelCaseStoreNames = camelCaseStoreNames;
            _collectionCommandExecuterJsInterop = new CollectionCommandExecuterJsInterop(moduleFactory);
            _staticCommandExecuterJsInterop = new StaticCommandExecuterJsInterop(moduleFactory);
        }

        protected List<DbVersionDefinition> InitStoresAndGetVersionDefinitions(
            IEnumerable<DbVersion> previousVersions,
            PropertyInfo[] properties,
            string? upgrade = null,
            string? upgradeModule = null)
        {
            var latestVersion = new DbVersionDefinition(VersionNumber, upgrade, upgradeModule);

            foreach (var property in properties)
            {
                if (typeof(IStore).IsAssignableFrom(property.PropertyType))
                {
                    var store = (IStore?)property.GetValue(this);

                    if (store != null)
                    {
                        var storeName = _camelCaseStoreNames ? Camelizer.ToCamelCase(property.Name) : property.Name;
                        store.Init(this, storeName, _collectionCommandExecuterJsInterop);
                        latestVersion.Stores.Add(new StoreDefinition(storeName, store.SchemaDefinitions));
                    }
                }
            }

            var versions = new List<DbVersionDefinition>() { latestVersion };

            versions.AddRange(previousVersions
                    .Select(v => v.GetDefinition(_camelCaseStoreNames))
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
        }

        public async Task Transaction(string mode, string[] storeNames, int timeout, Func<Task> transactionBody, CancellationToken cancellationToken = default)
        {
            var transformedStoreNames = _camelCaseStoreNames ? storeNames.Select(Camelizer.ToCamelCase).ToArray() : storeNames;
            var transactionBodyWrapper = new TransactionBodyWrapper(transactionBody);
            var command = new Command("transaction", DbJsReference, mode, transformedStoreNames, timeout, DotNetObjectReference.Create(transactionBodyWrapper));
            await _staticCommandExecuterJsInterop.ExecuteNonQuery(command, cancellationToken);
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

    /// <typeparam name="TConcrete">Concrete class that inherits form Db<TConcrete></typeparam>
    public abstract class Db<TConcrete> : Db
    {
        private static PropertyInfo[] Properties = typeof(TConcrete).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        protected Db(
            string databaseName,
            int currentVersionNumber,
            IEnumerable<DbVersion> previousVersions,
            IModuleFactory moduleFactory,
            string? upgrade = null,
            string? upgradeModule = null,
            bool camelCaseStoreNames = false) : base(databaseName, currentVersionNumber, moduleFactory, camelCaseStoreNames)
        {
            Versions = InitStoresAndGetVersionDefinitions(previousVersions, Properties, upgrade, upgradeModule);
        }
    }
}
