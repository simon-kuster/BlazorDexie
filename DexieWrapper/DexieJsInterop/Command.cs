namespace Nosthy.Blazor.DexieWrapper.DexieJsInterop
{
    public class Command
    {
        public string Cmd { get; }
        public object?[] DexieParameters { get; }
        public CommandParameters CommandParameters { get; }

        public Command(string command, object?[] dexieParameters, CommandParameters commandParameters)
        {
            Cmd = command;
            DexieParameters = dexieParameters;
            CommandParameters = commandParameters;
        }
    }
}
