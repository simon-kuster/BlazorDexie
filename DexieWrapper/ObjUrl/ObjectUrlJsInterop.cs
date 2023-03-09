using BlazorDexie.JsModule;

namespace BlazorDexie.ObjUrl
{
    public sealed class ObjectUrlJsInterop : IAsyncDisposable
    {
        private readonly IModule _module;
        private bool disposed = false;

        public bool _runInBrowser;

        public ObjectUrlJsInterop(IModuleFactory jsModuleFactory)
        {
            _module = jsModuleFactory.CreateModule("scripts/objectUrl.js");
            _runInBrowser = _module is EsModule;
        }

        public async Task<string> Create(byte[] data, string mimeType, CancellationToken cancellationToken = default)
        {
            return await _module.InvokeAsync<string>("createObjectUrlFromUint8Array", cancellationToken, data, mimeType);
        }

        public async Task Revoke(string objectUrl, CancellationToken cancellationToken = default)
        {
            await _module.InvokeAsync<string>("revokeObjectUrl", cancellationToken, objectUrl);
        }

        public async Task<byte[]> FetchData(string objectUrl, CancellationToken cancellationToken = default)
        {
            if (_runInBrowser)
            {
                return await _module.InvokeAsync<byte[]>("fetchObjectUrlAsUint8Array", cancellationToken, objectUrl);
            }

            var data = await _module.InvokeAsync<string>("fetchObjectUrlAsUint8Array", cancellationToken, objectUrl);
            return Convert.FromBase64String(data);
        }

        public async ValueTask DisposeAsync()
        {
            if (!disposed)
            {
                await _module.DisposeAsync();
                disposed = true;
            }
        }
    }
}
