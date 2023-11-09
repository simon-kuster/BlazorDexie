using BlazorDexie.JsModule;
using Jering.Javascript.NodeJS;
using System.IO;

namespace BlazorDexie.Test.ModuleWrappers
{
    public class CommonJsModuleFactory : IModuleFactory
    {
        private readonly INodeJSService _nodeJSService;
        private readonly string _basePath;
        private readonly string _userModulePathBase;

        public CommonJsModuleFactory(INodeJSService nodeJSService, string basePath, string userModulePathBase = "")
        {
            _nodeJSService = nodeJSService;
            _basePath = basePath;
            _userModulePathBase = userModulePathBase;
        }

        public IModule CreateModule(string modulePath)
        {
            return new CommonJsModule(_nodeJSService, Path.Combine(_basePath, modulePath), _userModulePathBase);
        }
    }
}
