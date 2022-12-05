using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.ObjUrl
{
    public class ObjectUrlJsInterop : IDisposable
    {
        private readonly IModule _module;
        private bool disposed = false;

        public bool _runInBrowser;

        public ObjectUrlJsInterop(IModuleFactory jsModuleFactory)
        {
            _module = jsModuleFactory.CreateModule("scripts/objectUrl.js");
            _runInBrowser = _module is EsModule;
        }

        public async Task<string> Create(byte[] data, CancellationToken cancellationToken = default)
        {
            return await _module.InvokeAsync<string>("createObjectUrlFromUint8Array", cancellationToken, data);
        }

        public async Task Revoke(string objectUrl, CancellationToken cancellationToken = default)
        {
            await _module.InvokeAsync<string>("revokeObjectUrl", cancellationToken, objectUrl);
        }

        public async Task<byte[]> FetchDataNode(string objectUrl, CancellationToken cancellationToken = default)
        {
            if (_runInBrowser)
            {
                return await _module.InvokeAsync<byte[]>("fetchObjectUrlAsUint8Array", cancellationToken, objectUrl);
            }

            var data =  await _module.InvokeAsync<string>("fetchObjectUrlAsUint8Array", cancellationToken, objectUrl);
            return Convert.FromBase64String(data);
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
                    _module.Dispose();
                }
            }
        }
    }
}
