namespace Nosthy.Blazor.DexieWrapper.JsModule
{
    public interface IModule
    {
        public Task<T> InvokeAsync<T>(string identifier, params object[] args);
        public Task InvokeVoidAsync(string identifier, params object[] args);
    }
}
