using System.Reflection;
using BlazorDexie.Definitions;
using BlazorDexie.Utils;

namespace BlazorDexie.Database
{
    public class DbVersion<TConcrete> : IDbVersion
    {
        private static readonly PropertyInfo[] Properties;
        private static readonly Dictionary<string, Func<object, object?>> PropertyGetterDictionary;

        private string? _upgrade;
        private string? _upgradeModule;

        public int VersionNumber { get; }

        static DbVersion()
        {
            Properties = typeof(TConcrete).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => typeof(IStore).IsAssignableFrom(p.PropertyType))
                .ToArray();

            PropertyGetterDictionary = Properties.ToDictionary(p => p.Name, p => PropertyAccessorDelegateBuilder.BuildPropertyGetter(p));
        }

        public DbVersion(int versionNumber, string? upgrade = null, string? upgradeModule = null)
        {
            VersionNumber = versionNumber;
            _upgrade = upgrade;
            _upgradeModule = upgradeModule;
        }

        public DbVersionDefinition GetDefinition(bool camelCaseStoreNames)
        {
            var currentVersion = new DbVersionDefinition(VersionNumber, _upgrade, _upgradeModule);

            foreach (var property in Properties)
            {
                var store = (IStore?)PropertyGetterDictionary[property.Name](this);
                if (store != null)
                {
                    var storeName = camelCaseStoreNames ? Camelizer.ToCamelCase(property.Name) : property.Name;
                    currentVersion.Stores.Add(new StoreDefinition(storeName, store.SchemaDefinitions));
                }
            }

            return currentVersion;
        }
    }
}
