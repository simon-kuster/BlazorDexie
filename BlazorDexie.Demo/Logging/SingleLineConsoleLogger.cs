namespace BlazorDexie.Demo.Logger
{
    public class SingleLineConsoleLogger : ILogger
    {
        private readonly string _categoryName;

        public SingleLineConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (formatter != null)
            {
                var message = formatter(state, exception);
                if (exception != null)
                {
                    message += $" | Exception: {exception.Message}";
                }

                // Ensure a single-line log
                message = message.Replace(Environment.NewLine, " ").Replace("\n", " ");
                Console.WriteLine($"{logLevel}: [{_categoryName}] {message}");
            }
        }
    }
}
