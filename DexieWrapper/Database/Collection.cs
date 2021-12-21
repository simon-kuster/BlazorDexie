using DexieWrapper.Definitions;
using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public class Collection<T>
    {
        protected DbDefinition DbDefinition = null!;
        protected CommandExecuterJsInterop CommandExecuterJsInterop = null!;
        protected string StoreName = null!;
        protected virtual List<Command> CurrentCommands { get; } = new List<Command>();

        protected Collection()
        {
        }

        public Collection(DbDefinition dbDefinition, string storeName, CommandExecuterJsInterop commandExecuterJsInterop)
        {
            Init(dbDefinition, storeName, commandExecuterJsInterop);
        }

        public void Init(DbDefinition dbDefinition, string storeName, CommandExecuterJsInterop commandExecuterJsInterop)
        {
            DbDefinition = dbDefinition;
            StoreName = storeName;
            CommandExecuterJsInterop = commandExecuterJsInterop;
        }

        public void AddCommand(string command, params object?[] parameters)
        {
            CurrentCommands.Add(new Command(command, parameters));
        }

        public async Task<T[]> ToArray()
        {
            return await Execute<T[]>("toArray");
        }

        public async Task<List<T>> ToList()
        {
            return await Execute<List<T>>("toArray");
        }

        protected async Task<TRet> Execute<TRet>(string command, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            return await CommandExecuterJsInterop.Execute<TRet>(DbDefinition, StoreName, commands);
        }

        protected async Task ExecuteNonQuery(string command, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            await CommandExecuterJsInterop.ExecuteNonQuery(DbDefinition, StoreName, commands);
        }
    }
}
