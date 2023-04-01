namespace BlazorDexie.Definitions
{
    public class DbVersionDefinition
    {
        public int VersionNumber { get; }
        public string? Upgrade { get; }
        public string? UpgradeModule { get; }

        public List<StoreDefinition> Stores { get; set; } = new();

        public DbVersionDefinition(int versionNumber, string? upgrade, string? upgradeModule)
        {
            VersionNumber = versionNumber;
            Upgrade = upgrade;
            UpgradeModule = upgradeModule;
        }
    }
}
