namespace DexieWrapper.JsInterop
{
    public class Command
    {
        public string Cmd { get; }
        public List<object?> Parameters { get; }
        public List<Command> SubCommands { get; } = new List<Command>();

        public Command(string command, List<object?> parameters)
        {
            Cmd = command;
            Parameters = parameters;
        }

        public void AddCommand(string command, List<object?> parameters)
        {
            SubCommands.Add(new Command(command, parameters));
        }
    }
}
