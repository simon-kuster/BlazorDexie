using Microsoft.JSInterop;

namespace Nosthy.Blazor.DexieWrapper.JsModule
{
    public class JsObjectReferenceWrapper : IJsModule
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public JsObjectReferenceWrapper(IJSRuntime jsRuntime, string modulePath)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", modulePath).AsTask());
        }

        public async Task<T> InvokeAsync<T>(string identifier, params object[] args)
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<T>(identifier, args);
        }

        public async Task InvokeVoidAsync(string identifier, params object[] args)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync(identifier, args);
        }
    }
}
