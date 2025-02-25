using System.Reflection;
using BlazorDexie.Definitions;
using BlazorDexie.Utils;

namespace BlazorDexie.Database
{
    public class DbVersion
    {
        private string? _upgrade;
        private string? _upgradeModule;
        private PropertyInfo[]? _properties;
        private Dictionary<Type, Func<object, object?>>? _propertyGetterDictionary;

        public int VersionNumber { get; }

        [Obsolete("Use DbVersion<TConcrete> instead for better performance")]
        public DbVersion(int versionNumber, string? upgrade = null, string? upgradeModule = null)
        {
            VersionNumber = versionNumber;
            _upgrade = upgrade;
            _upgradeModule = upgradeModule;
        }

        public DbVersion(int versionNumber, PropertyInfo[] properties, Dictionary<Type, Func<object, object?>> propertyGetterDictionary, string? upgrade = null, string? upgradeModule = null)
        {
            VersionNumber = versionNumber;
            _properties = properties;
            _propertyGetterDictionary = propertyGetterDictionary;
            _upgrade = upgrade;
            _upgradeModule = upgradeModule;
        }

        public DbVersionDefinition GetDefinition(bool camelCaseStoreNames)
        {
            var currentVersion = new DbVersionDefinition(VersionNumber, _upgrade, _upgradeModule);

            var properties = _properties ?? GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                if (typeof(IStore).IsAssignableFrom(property.PropertyType))
                {
                    var store = _propertyGetterDictionary != null
                        ? (IStore?)_propertyGetterDictionary[property.PropertyType](this)
                        : (IStore?)property.GetValue(this);

                    if (store != null)
                    {
                        var storeName = camelCaseStoreNames ? Camelizer.ToCamelCase(property.Name) : property.Name;
                        currentVersion.Stores.Add(new StoreDefinition(storeName, store.SchemaDefinitions));
                    }
                }
            }

            return currentVersion;
        }
    }


    public class DbVersion<TConcrete> : DbVersion
    {
        private static readonly PropertyInfo[] Properties;
        private static readonly Dictionary<Type, Func<object, object?>> PropertyGetterDictionary;

        public DbVersion(int versionNumber, string? upgrade = null, string? upgradeModule = null)
            : base(versionNumber, Properties, PropertyGetterDictionary, upgrade, upgradeModule)
        {
        }

        static DbVersion()
        {
            Properties = typeof(TConcrete).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyGetterDictionary = Properties.ToDictionary(p => p.PropertyType, p => PropertyAccessorDelegateBuilder.BuildPropertyGetter(p));
        }
    }
}
