namespace DexieWrapper.JsModule
{
    public interface IJsModule
    {
        public Task<T> InvokeAsync<T>(string identifier, params object[] args);
        public Task InvokeVoidAsync(string identifier, params object[] args);
    }
}
