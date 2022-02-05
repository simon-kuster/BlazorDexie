using Jering.Javascript.NodeJS;
using Nosthy.Blazor.DexieWrapper.JsModule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers
{
    public class ModuleWrapper : IJsModule
    {
        INodeJSService _nodeJSService;
        private const string ModuleWrapperPath = "../../../wwwroot/scripts/es-module-wrapper.js";
        private string _modulePath;

        public ModuleWrapper(INodeJSService nodeJSService, string modulePath)
        {
            _nodeJSService = nodeJSService;
            _modulePath = modulePath;
        }

        public async Task<T> InvokeAsync<T>(string identifier, params object[] args)
        {
            var argList = new List<object?> { _modulePath, identifier };
            argList.AddRange(args);

#pragma warning disable CS8603 // Possible null reference return.
            return await _nodeJSService.InvokeFromFileAsync<T>(ModuleWrapperPath, args: argList.ToArray());
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task InvokeVoidAsync(string identifier, params object[] args)
        {
            var argList = new List<object?> { _modulePath, identifier };
            argList.AddRange(args);

            await _nodeJSService.InvokeFromFileAsync(ModuleWrapperPath, args: argList.ToArray());
        }
    }
}
