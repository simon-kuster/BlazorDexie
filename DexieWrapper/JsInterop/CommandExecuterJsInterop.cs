using DexieWrapper.Definitions;
using DexieWrapper.JsModule;

namespace DexieWrapper.JsInterop
{
    public class CommandExecuterJsInterop
    {
        private readonly IJsModule _jsModule;

        public CommandExecuterJsInterop(IJsModuleFactory jsModuleFactory)
        {
            _jsModule = jsModuleFactory.CreateModule("commandExecuter.js");
        }

        public async Task<T> Execute<T>(DbDefinition databaseDefinition, Command command)
        {
            return await _jsModule.InvokeAsync<T>("execute", databaseDefinition, command);
        }

        public async Task ExecuteNonQuery(DbDefinition databaseDefinition, Command command)
        {
            await _jsModule.InvokeVoidAsync("executeNonQuery", databaseDefinition, command);
        }
    }
}
