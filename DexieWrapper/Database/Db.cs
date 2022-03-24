using Microsoft.JSInterop;
using Nosthy.Blazor.DexieWrapper.Definitions;
using Nosthy.Blazor.DexieWrapper.JsInterop;
using Nosthy.Blazor.DexieWrapper.JsModule;
using System.Text.Json.Serialization;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public abstract class Db
    {
        private readonly CommandExecuterJsInterop _commandExecuterJsInterop;

        public string DatabaseName { get; }
        public int VersionNumber { get; }
        public DbDefinition DbDefinition { get; }
        public IJSObjectReference? DbRef { get; private set; }

        [JsonIgnore]
        public IEnumerable<DbVersion> PreviousVersions { get; }

        public Db(string databaseName, int currentVersionNumber, IEnumerable<DbVersion> previousVersions, IModuleFactory jsModuleFactory)
        {
            DatabaseName = databaseName;
            VersionNumber = currentVersionNumber;
            PreviousVersions = previousVersions;
            _commandExecuterJsInterop = new CommandExecuterJsInterop(jsModuleFactory);

            var latestVersion = new DbVersionDefinition(VersionNumber);
            DbDefinition = new DbDefinition(DatabaseName);

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

            DbDefinition.Versions.Add(latestVersion);

            foreach (var version in PreviousVersions)
            {
                DbDefinition.Versions.Add(version.GetDefinition());
            }
        }

        public async Task Init(CancellationToken cancellationToken)
        {
            if (DbRef == null && _commandExecuterJsInterop.CanUseObjectReference)
            {
                // Optimized code for Blazor
                // Create Dexie object only once
                DbRef = await _commandExecuterJsInterop.InitDb(DbDefinition, cancellationToken);
            }
        }
    }
}
