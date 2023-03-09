using Microsoft.JSInterop;

namespace BlazorDexie.JsModule
{
    public class EsModuleFactory : IModuleFactory
    {
        private const string BasePath = "./_content/Nosthy.Blazor.DexieWrapper";
        private IJSRuntime _jsRuntime;

        public EsModuleFactory(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public IModule CreateModule(string modulePath)
        {
            return new EsModule(_jsRuntime, Path.Combine(BasePath, modulePath));
        }
    }
}
