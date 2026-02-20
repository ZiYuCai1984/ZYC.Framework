namespace ZYC.Framework.Abstractions;

/// <summary>
///     Represents a typed application logger.
/// </summary>
/// <typeparam name="T">The type that owns the logger.</typeparam>
// ReSharper disable once UnusedTypeParameter
public interface IAppLogger<T> : IAppLogger
{
}

/// <summary>
///     Represents application logging operations.
/// </summary>
public interface IAppLogger
{
    /// <summary>
    ///     Writes an informational log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Info(string message)
    {
    }

    /// <summary>
    ///     Writes a debug log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Debug(string message)
    {
    }

    /// <summary>
    ///     Writes an error log message for an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    void Error(Exception exception)
    {
    }

    /// <summary>
    ///     Writes an error log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Error(string message)
    {
    }

    /// <summary>
    ///     Writes a warning log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Warn(string message)
    {
    }

    /// <summary>
    ///     Gets the underlying Microsoft.Extensions.Logging logger, if available.
    /// </summary>
    /// <returns>The underlying logger instance.</returns>
    object? GetMicrosoftExtensionsLogger()
    {
        return null;
    }
}