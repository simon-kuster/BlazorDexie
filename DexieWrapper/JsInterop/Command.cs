namespace DexieWrapper.JsInterop
{
    public class Command
    {
        public string Cmd { get; }
        public object?[] Parameters { get; }

        public Command(string command, params object?[] parameters)
        {
            Cmd = command;
            Parameters = parameters;
        }
    }
}
