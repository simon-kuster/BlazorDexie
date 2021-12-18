using DexieWrapper.Definitions;
using DexieWrapper.JsInterop;
using DexieWrapper.JsModule;
using System.Text.Json.Serialization;

namespace DexieWrapper.Database
{
    public abstract class Db
    {
        private readonly CommandExecuterJsInterop _commandExecuterJsInterop;

        public string DatabaseName { get; }
        public int VersionNumber { get; } 

        [JsonIgnore]
        public IEnumerable<DbVersion> PreviousVersions { get; }

        public Db(string databaseName, int currentVersionNumber, IEnumerable<DbVersion> previousVersions, IJsModuleFactory jsModuleFactory)
        {
            DatabaseName = databaseName;
            VersionNumber = currentVersionNumber;
            PreviousVersions = previousVersions;
            _commandExecuterJsInterop = new CommandExecuterJsInterop(jsModuleFactory);

            Init();
        }

        private void Init()
        {
            var latestVersion = new DbVersionDefinition(VersionNumber);
            var dbDefinition = new DbDefinition(DatabaseName);

            var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                if (typeof(IStore).IsAssignableFrom(property.PropertyType))
                {
                    var store = (IStore?)property.GetValue(this);

                    if (store != null)
                    {
                        var storeName = property.Name;
                        store.Init(dbDefinition, storeName, _commandExecuterJsInterop);
                        latestVersion.Stores.Add(new StoreDefinition(storeName, store.Indices));
                    }
                }
            }
            
            dbDefinition.Versions.Add(latestVersion);

            foreach (var version in PreviousVersions)
            {
                dbDefinition.Versions.Add(version.GetDefinition());
            }
        }
    }
}
