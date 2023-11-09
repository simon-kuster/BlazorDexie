using Microsoft.JSInterop;

namespace BlazorDexie.JsModule
{
    public class EsModuleFactory : IModuleFactory
    {
        private const string BasePath = "./_content/BlazorDexie";
        private IJSRuntime _jsRuntime;
        private readonly string _userModulePathBase;

        public EsModuleFactory(IJSRuntime jsRuntime, string userModulePathBase)
        {
            _jsRuntime = jsRuntime;
            _userModulePathBase = userModulePathBase;
        }

        public IModule CreateModule(string modulePath)
        {
            return new EsModule(_jsRuntime, Path.Combine(BasePath, modulePath), _userModulePathBase);
        }
    }
}
