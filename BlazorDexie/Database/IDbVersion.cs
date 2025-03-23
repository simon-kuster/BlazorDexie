using BlazorDexie.Definitions;

namespace BlazorDexie.Database
{
    public interface IDbVersion
    {
        int VersionNumber { get; }

        DbVersionDefinition GetDefinition(bool camelCaseStoreNames);
    }
}