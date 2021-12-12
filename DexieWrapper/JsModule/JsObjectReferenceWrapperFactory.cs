using Microsoft.JSInterop;

namespace DexieWrapper.JsModule
{
    public class JsObjectReferenceWrapperFactory : IJsModuleFactory
    {
        private readonly string _basePath;
        private IJSRuntime _jsRuntime;

        public JsObjectReferenceWrapperFactory(IJSRuntime jsRuntime, string basePath)
        {
            _jsRuntime = jsRuntime;
            _basePath = basePath;
        }

        public IJsModule CreateModule(string modulePath)
        {
            return new JsObjectReferenceWrapper(_jsRuntime, Path.Combine(_basePath, modulePath));
        }
    }
}
