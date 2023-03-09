namespace BlazorDexie.Definitions
{
    public class DbDefinition
    {
        public string DatabaseName { get; }
        public List<DbVersionDefinition> Versions { get; } = new();

        public DbDefinition(string databaseName)
        {
            DatabaseName = databaseName;
        }
    }
}
