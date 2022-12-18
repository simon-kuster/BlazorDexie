using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.ObjUrl
{
    public sealed class ObjectUrlService : IAsyncDisposable
    {
        private readonly ObjectUrlJsInterop _objecUrlJsInterop;
        private bool disposed = false;

        public ObjectUrlService(IModuleFactory jsModuleFactory)
        {
            _objecUrlJsInterop = new ObjectUrlJsInterop(jsModuleFactory);
        }

        public async Task<string> Create(byte[] data, string mimeType = "", CancellationToken cancellationToken = default)
        {
            return await _objecUrlJsInterop.Create(data, mimeType, cancellationToken);
        }

        public async Task Revoke(string objectUrl, CancellationToken cancellationToken = default)
        {
            await _objecUrlJsInterop.Revoke(objectUrl, cancellationToken);
        }

        public async Task<byte[]> FetchData(string objectUrl, CancellationToken cancellationToken = default)
        {
           return await _objecUrlJsInterop.FetchData(objectUrl, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (!disposed)
            {
                await _objecUrlJsInterop.DisposeAsync();
                disposed = true;
            }
        }
    }
}

