namespace DexieWrapper.JsInterop
{
    public class MainCommand : Command
    {
        public string StoreName { get; }
        
        public MainCommand(string storeName, string command, List<object?> parameters) : base(command, parameters)
        {
            StoreName = storeName;
        }
    }
}
