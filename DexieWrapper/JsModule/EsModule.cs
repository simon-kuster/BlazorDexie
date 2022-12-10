using Microsoft.JSInterop;

namespace Nosthy.Blazor.DexieWrapper.JsModule
{
    public sealed class EsModule : IModule
    {
        private Lazy<Task<IJSObjectReference>>? _jsObjectReferenceTask;

        public EsModule(IJSRuntime jsRuntime, string modulePath)
        {
            _jsObjectReferenceTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", modulePath).AsTask());
        }

        public async Task<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, params object[] args)
        {
            var jsObjectReference = await GetJsObjectReference();
            return await jsObjectReference.InvokeAsync<T>(identifier, cancellationToken, args);
        }

        public async Task InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object[] args)
        {
            var jsObjectReference = await GetJsObjectReference();
            await jsObjectReference.InvokeVoidAsync(identifier, cancellationToken, args);
        }

        public async ValueTask DisposeAsync()
        {
            if(_jsObjectReferenceTask?.IsValueCreated == true)
            {
                var jsObjectReference = await GetJsObjectReference();

                await jsObjectReference.DisposeAsync().ConfigureAwait(false); 
                _jsObjectReferenceTask = null;
            }
        }

        private async Task<IJSObjectReference> GetJsObjectReference()
        {
            if (_jsObjectReferenceTask == null)
            {
                throw new ObjectDisposedException("JsObjectReference is disposed");
            }

            return await _jsObjectReferenceTask.Value;
        }
    }
}
