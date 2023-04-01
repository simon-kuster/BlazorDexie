using BlazorDexie.Definitions;

namespace BlazorDexie.Database
{
    public class DbVersion
    {
        private string? _upgrade; 
        string? _upgradeModule;

        public int VersionNumber { get; }

        public DbVersion(int versionNumber, string? upgrade = null, string? upgradeModule = null)
        {
            VersionNumber = versionNumber;
            _upgrade = upgrade;
            _upgradeModule = upgradeModule; 
        }

        public DbVersionDefinition GetDefinition()
        {
            var currentVersion = new DbVersionDefinition(VersionNumber, _upgrade, _upgradeModule);

            var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                if (typeof(IStore).IsAssignableFrom(property.PropertyType))
                {
                    var store = (IStore?)property.GetValue(this);

                    if (store != null)
                    {
                        var storeName = property.Name;
                        currentVersion.Stores.Add(new StoreDefinition(storeName, store.SchemaDefinitions));
                    }
                }
            }

            return currentVersion;
        }
    }
}
