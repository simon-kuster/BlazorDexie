using Microsoft.JSInterop;
using Nosthy.Blazor.DexieWrapper.Definitions;
using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.JsInterop
{
    public class CommandExecuterJsInterop
    {
        private readonly IJsModule _jsModule;

        public bool CanUseObjectReference { get; set; }

        public CommandExecuterJsInterop(IJsModuleFactory jsModuleFactory)
        {
            _jsModule = jsModuleFactory.CreateModule("scripts/commandExecuter.js");
            CanUseObjectReference = _jsModule is JsObjectReferenceWrapper;
        }

        public async Task<T> InitDbAndExecute<T>(DbDefinition dbDefinition, string storeName, List<Command> commands)
        {
            return await _jsModule.InvokeAsync<T>("initDbAndExecute", dbDefinition, storeName, commands);
        }

        public async Task<T> Execute<T>(IJSObjectReference dbJsObjectRef, string storeName, List<Command> commands)
        {
            return await _jsModule.InvokeAsync<T>("execute", dbJsObjectRef, storeName, commands);
        }

        public async Task InitDbAndExecuteNonQuery(DbDefinition dbDefinition, string storeName, List<Command> commands)
        {
            await _jsModule.InvokeVoidAsync("initDbAndExecuteNonQuery", dbDefinition, storeName, commands);
        }

        public async Task ExecuteNonQuery(IJSObjectReference dbJsObjectRef, string storeName, List<Command> commands)
        {
            await _jsModule.InvokeVoidAsync("executeNonQuery", dbJsObjectRef, storeName, commands);
        }

        public async Task<IJSObjectReference> InitDb(DbDefinition dbDefinition)
        {
            return await _jsModule.InvokeAsync<IJSObjectReference>("initDb", dbDefinition);
        }
    }
}
