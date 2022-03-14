using Nosthy.Blazor.DexieWrapper.Definitions;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public class DbVersion
    {
        public int VersionNumber { get; }

        public DbVersion(int versionNumber)
        {
            VersionNumber = versionNumber;
        }

        public DbVersionDefinition GetDefinition()
        {
            var currentVersion = new DbVersionDefinition(VersionNumber);

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
