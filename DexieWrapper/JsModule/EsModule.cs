using Microsoft.JSInterop;

namespace Nosthy.Blazor.DexieWrapper.JsModule
{
    public class EsModule : IModule
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public EsModule(IJSRuntime jsRuntime, string modulePath)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", modulePath).AsTask());
        }

        public async Task<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, params object[] args)
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<T>(identifier, cancellationToken, args);
        }

        public async Task InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object[] args)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync(identifier, cancellationToken, args);
        }
    }
}
