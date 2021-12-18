namespace DexieWrapper.JsInterop
{
    public class Command
    {
        public string StoreName { get; }
        public string Cmd { get; }
        public List<object?> Parameters { get; }

        public Command(string storeName, string command, List<object?> parameters)
        {
            StoreName = storeName;
            Cmd = command;
            Parameters = parameters;
        }
    }
}
