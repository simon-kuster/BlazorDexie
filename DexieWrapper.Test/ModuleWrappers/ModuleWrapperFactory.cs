using Jering.Javascript.NodeJS;
using Nosthy.Blazor.DexieWrapper.JsModule;
using System.IO;

namespace Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers
{
    public class ModuleWrapperFactory : IJsModuleFactory
    {
        private readonly INodeJSService _nodeJSService;
        private readonly string _basePath;

        public ModuleWrapperFactory(INodeJSService nodeJSService, string basePath)
        {
            _nodeJSService = nodeJSService;
            _basePath = basePath;
        }

        public IJsModule CreateModule(string modulePath)
        {
            return new ModuleWrapper(_nodeJSService, Path.Combine(_basePath, modulePath));
        }
    }
}
