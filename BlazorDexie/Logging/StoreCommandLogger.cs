using BlazorDexie.JsInterop;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BlazorDexie.Logging
{
    public class StoreCommandLogger
    {
        private Stopwatch? _stopwatch;
        private ILogger _logger;
        private readonly LogLevel _logLevel;

        public StoreCommandLogger(ILogger logger, LogLevel logLevel)
        {
            _logger = logger;
            _logLevel = logLevel;
        }

        public void Start()
        {
            if (_logger.IsEnabled(_logLevel))
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }
        }

        public void Log(string storeName, List<Command> commands)
        {
            if (_logger.IsEnabled(_logLevel))
            {
                var commandLogMessage = string.Join(' ', commands.Select(c => $"{c.Cmd}({string.Join(", ", c.Parameters)})"));
                var message = $"Store {storeName}.{string.Join(' ', commandLogMessage)}";

                _stopwatch?.Stop();
                if (_stopwatch != null)
                {
                    message += $" [{_stopwatch.Elapsed.TotalMilliseconds}ms]";
                }

                _logger.LogInformation(message);
            }
        }
    }
}
