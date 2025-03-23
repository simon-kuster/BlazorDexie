using BlazorDexie.Demo.Logger;

namespace BlazorDexie.Demo.Logging
{
    public class SingleLineConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new SingleLineConsoleLogger(categoryName);

        public void Dispose() { }
    }
}
