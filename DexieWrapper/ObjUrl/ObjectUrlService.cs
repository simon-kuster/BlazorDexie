using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.ObjUrl
{
    public class ObjectUrlService : IDisposable
    {
        private readonly ObjectUrlJsInterop _objecUrlJsInterop;
        private bool disposed = false;

        public ObjectUrlService(IModuleFactory jsModuleFactory)
        {
            _objecUrlJsInterop = new ObjectUrlJsInterop(jsModuleFactory);
        }

        public async Task<string> Create(byte[] data, CancellationToken cancellationToken = default)
        {
            return await _objecUrlJsInterop.Create(data, cancellationToken);
        }

        public async Task Revoke(string objectUrl, CancellationToken cancellationToken = default)
        {
            await _objecUrlJsInterop.Revoke(objectUrl, cancellationToken);
        }

        public async Task<byte[]> FetchDataNode(string objectUrl, CancellationToken cancellationToken = default)
        {
           return await _objecUrlJsInterop.FetchDataNode(objectUrl, cancellationToken);
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
                    _objecUrlJsInterop.Dispose();
                }
            }
        }
    }
}

