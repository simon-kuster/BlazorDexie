using DexieWrapper.Definitions;
using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public class Collection<T>
    {
        protected DbDefinition _dbDefinition = null!;
        protected CommandExecuterJsInterop _commandExecuterJsInterop = null!;
        protected string _storeName = null!;

        public MainCommand? CurrentCommand { get; set; }

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
            if (CurrentCommand == null)
            {
                CurrentCommand = new MainCommand(_storeName, "toArray", new List<object?>());
            }
            else
            {
                CurrentCommand.SubCommands.Add(new Command("toArray", new List<object?> { }));
            }

            return await _commandExecuterJsInterop.Execute<T[]?>(_dbDefinition, CurrentCommand);
        }
    }
}
