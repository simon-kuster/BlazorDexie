using DexieWrapper.Definitions;
using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public class Collection<T>
    {
        protected DbDefinition _dbDefinition = null!;
        protected CommandExecuterJsInterop _commandExecuterJsInterop = null!;
        protected string _storeName = null!;

        public virtual List<Command> CurrentCommands { get; } = new List<Command>();

        public Collection()
        {
        }

        public Collection(DbDefinition dbDefinition, string storeName, CommandExecuterJsInterop commandExecuterJsInterop)
        {
            Init(dbDefinition, storeName, commandExecuterJsInterop);
        }

        public void Init(DbDefinition dbDefinition, string storeName, CommandExecuterJsInterop commandExecuterJsInterop)
        {
            _dbDefinition = dbDefinition;
            _storeName = storeName;
            _commandExecuterJsInterop = commandExecuterJsInterop;
        }

        public async Task<T[]?> ToArray()
        {
            return await Execute<T[]?>(new Command("toArray", new List<object?> { }));
        }

        public void AddCommand(Command command)
        {
            CurrentCommands.Add(command);
        }

        protected async Task<TRet> Execute<TRet>(Command command)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(command);

            return await _commandExecuterJsInterop.Execute<TRet>(_dbDefinition, _storeName, commands);
        }

        protected async Task ExecuteNonQuery(Command command)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(command);

            await _commandExecuterJsInterop.ExecuteNonQuery(_dbDefinition, _storeName, commands);
        }
    }
}
