using DexieWrapper.Definitions;
using DexieWrapper.JsInterop;
using DexieWrapper.JsModule;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace DexieWrapper.Database
{
    public abstract class Db
    {
        private readonly CommandExecuterJsInterop _commandExecuterJsInterop;

        public string DatabaseName { get; }
        public int VersionNumber { get; } 
        public DbDefinition DbDefinition { get; }
        public IJSObjectReference? DbJsObjectRef { get; private set; }

        [JsonIgnore]
        public IEnumerable<DbVersion> PreviousVersions { get; }

        public Db(string databaseName, int currentVersionNumber, IEnumerable<DbVersion> previousVersions, IJsModuleFactory jsModuleFactory)
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
                        latestVersion.Stores.Add(new StoreDefinition(storeName, store.Indices));
                    }
                }
            }

            DbDefinition.Versions.Add(latestVersion);

            foreach (var version in PreviousVersions)
            {
                DbDefinition.Versions.Add(version.GetDefinition());
            }
        }

        public async Task Init()
        {
            if (DbJsObjectRef == null && _commandExecuterJsInterop.CanUseObjectReference)
            {
                // Optimized code for Blazor
                // Create Dexie object only once
                DbJsObjectRef = await _commandExecuterJsInterop.InitDb(DbDefinition);
            }
        }
    }
}
