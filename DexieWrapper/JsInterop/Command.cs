namespace DexieWrapper.JsInterop
{
    public class Command
    {
        public string Cmd { get; }
        public List<object?> Parameters { get; }

        public Command(string command, List<object?> parameters)
        {
            Cmd = command;
            Parameters = parameters;
        }
    }
}
