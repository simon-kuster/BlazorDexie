namespace BlazorDexie.JsModule
{
    public interface IModule : IAsyncDisposable
    {
        public string GetUserModuleBasePath();
        public Task<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, params object[] args);
        public Task InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object[] args);
    }
}
