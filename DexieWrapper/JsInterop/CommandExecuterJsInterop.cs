using DexieWrapper.Definitions;
using DexieWrapper.JsModule;

namespace DexieWrapper.JsInterop
{
    public class CommandExecuterJsInterop
    {
        private readonly IJsModule _jsModule;

        public CommandExecuterJsInterop(IJsModuleFactory jsModuleFactory)
        {
            _jsModule = jsModuleFactory.CreateModule("scripts/commandExecuter.js");
        }

        public async Task<T> Execute<T>(DbDefinition databaseDefinition, string storeName, List<Command> commands)
        {
            return await _jsModule.InvokeAsync<T>("execute", databaseDefinition, storeName, commands);
        }

        public async Task ExecuteNonQuery(DbDefinition databaseDefinition, string storeName, List<Command> commands)
        {
            await _jsModule.InvokeVoidAsync("executeNonQuery", databaseDefinition, storeName, commands);
        }
    }
}
