using BlazorDexie.JsModule;
using Jering.Javascript.NodeJS;
using System.IO;

namespace BlazorDexie.Test.ModuleWrappers
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
