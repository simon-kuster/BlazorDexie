namespace BlazorDexie.Definitions
{
    public class DbVersionDefinition
    {
        public int VersionNumber { get; }
        public List<StoreDefinition> Stores { get; set; } = new();

        public DbVersionDefinition(int versionNumber)
        {
            VersionNumber = versionNumber;
        }
    }
}
