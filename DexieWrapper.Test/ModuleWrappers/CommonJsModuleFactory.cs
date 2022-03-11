using Jering.Javascript.NodeJS;
using Nosthy.Blazor.DexieWrapper.JsModule;
using System.IO;

namespace Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers
{
    public class CommonJsModuleFactory : IModuleFactory
    {
        private readonly INodeJSService _nodeJSService;
        private readonly string _basePath;

        public CommonJsModuleFactory(INodeJSService nodeJSService, string basePath)
        {
            _nodeJSService = nodeJSService;
            _basePath = basePath;
        }

        public IModule CreateModule(string modulePath)
        {
            return new CommonJsModule(_nodeJSService, Path.Combine(_basePath, modulePath));
        }
    }
}
