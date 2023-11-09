using Microsoft.JSInterop;

namespace BlazorDexie.JsModule
{
    public sealed class EsModule : IModule
    {
        private Lazy<Task<IJSObjectReference>>? _jsObjectReferenceTask;
        private CancellationTokenSource _internalCancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _internalCancellationToken;
        private readonly string _userModuleBasePath;

        public EsModule(IJSRuntime jsRuntime, string modulePath, string userModuleBasePath)
        {
            _internalCancellationToken = _internalCancellationTokenSource.Token;
            _jsObjectReferenceTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", modulePath).AsTask());
            _userModuleBasePath = userModuleBasePath;
        }

        public string GetUserModuleBasePath()
        {
            return _userModuleBasePath;
        }

        public async Task<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, params object[] args)
        {
            var combinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_internalCancellationToken, cancellationToken).Token;
            var jsObjectReference = await GetJsObjectReference(combinedCancellationToken);
            return await jsObjectReference.InvokeAsync<T>(identifier, combinedCancellationToken, args);
        }

        public async Task InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object[] args)
        {
            var combinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_internalCancellationToken, cancellationToken).Token;
            var jsObjectReference = await GetJsObjectReference(combinedCancellationToken);
            await jsObjectReference.InvokeVoidAsync(identifier, combinedCancellationToken, args);
        }

        public async ValueTask DisposeAsync()
        {
            if (_jsObjectReferenceTask?.IsValueCreated == true && !_internalCancellationToken.IsCancellationRequested)
            {
                var jsObjectReference = await GetJsObjectReference(_internalCancellationToken);

                await jsObjectReference.DisposeAsync().ConfigureAwait(false);
                _jsObjectReferenceTask = null;
            }

            _internalCancellationTokenSource.Cancel();
        }

        private async Task<IJSObjectReference> GetJsObjectReference(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            if (_jsObjectReferenceTask == null)
            {
                throw new ObjectDisposedException("JsObjectReference is disposed");
            }

            return await _jsObjectReferenceTask.Value;
        }
    }
}
