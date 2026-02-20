using log4net;
using log4net.Core;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ZYC.Framework.Modules.Log;

internal sealed class Log4NetLogger : ILogger
{
    private readonly ILog _log;

    public Log4NetLogger(ILog log)
    {
        _log = log;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return NullScope.Instance;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => _log.IsDebugEnabled,
            LogLevel.Debug => _log.IsDebugEnabled,
            LogLevel.Information => _log.IsInfoEnabled,
            LogLevel.Warning => _log.IsWarnEnabled,
            LogLevel.Error => _log.IsErrorEnabled,
            LogLevel.Critical => _log.IsFatalEnabled,
            _ => false
        };
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);

        _log.Logger.Log(
            typeof(Log4NetLogger),
            MapLevel(logLevel),
            message,
            exception);
    }

    private static Level MapLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => Level.Debug,
            LogLevel.Debug => Level.Debug,
            LogLevel.Information => Level.Info,
            LogLevel.Warning => Level.Warn,
            LogLevel.Error => Level.Error,
            LogLevel.Critical => Level.Fatal,
            _ => Level.Off
        };
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}