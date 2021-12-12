using DexieWrapper.JsModule;
using Jering.Javascript.NodeJS;
using System.IO;

namespace DexieWrapper.Test.ModuleWrappers
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
