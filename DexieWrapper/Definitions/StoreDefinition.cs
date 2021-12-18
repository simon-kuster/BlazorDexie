namespace DexieWrapper.Definitions
{
    public class StoreDefinition
    {
        public string StoreName { get; }
        public string Indices { get; }

        public StoreDefinition(string storeName, string[] indices)
        {
            StoreName = storeName;
            Indices = string.Join(",", indices);
        }
    }
}
