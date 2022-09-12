using Jering.Javascript.NodeJS;
using Nosthy.Blazor.DexieWrapper.JsModule;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers
{
    public class CommonJsModule : IModule
    {
        INodeJSService _nodeJSService;
        private const string ModuleWrapperPath = "../../../wwwroot/scripts/es-module-wrapper.js";
        private string _modulePath;
        private bool disposed = false;

        public CommonJsModule(INodeJSService nodeJSService, string modulePath)
        {
            _nodeJSService = nodeJSService;
            _modulePath = modulePath;
        }

        public async Task<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, params object[] args)
        {
            var argList = new List<object?> { _modulePath, identifier };
            argList.AddRange(args);

#pragma warning disable CS8603 // Possible null reference return.
            return await _nodeJSService.InvokeFromFileAsync<T>(ModuleWrapperPath, args: argList.ToArray(), cancellationToken: cancellationToken);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object[] args)
        {
            var argList = new List<object?> { _modulePath, identifier };
            argList.AddRange(args);

            await _nodeJSService.InvokeFromFileAsync(ModuleWrapperPath, args: argList.ToArray(), cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _nodeJSService.Dispose();
                }
            }
        }
    }
}
