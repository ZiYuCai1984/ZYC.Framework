using Microsoft.Extensions.Logging;

namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides compatibility extension members for <see cref="ILogger" /> to simplify logging calls.
///     This mimics the API of older logging frameworks (like log4net or NLog) while wrapping the standard Microsoft
///     Logging abstractions.
/// </summary>
public static class AppLoggerCompatExtensions
{
    extension(ILogger logger)
    {
        /// <summary>
        ///     Logs a message at the <see cref="LogLevel.Information" /> level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Info(string message)
        {
            logger.LogInformation(message);
        }

        /// <summary>
        ///     Logs a message at the <see cref="LogLevel.Debug" /> level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Debug(string message)
        {
            logger.LogDebug(message);
        }

        /// <summary>
        ///     Logs a message at the <see cref="LogLevel.Warning" /> level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Warn(string message)
        {
            logger.LogWarning(message);
        }

        /// <summary>
        ///     Logs a message at the <see cref="LogLevel.Error" /> level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Error(string message)
        {
            logger.LogError(message);
        }

        /// <summary>
        ///     Logs an exception and an optional message at the <see cref="LogLevel.Error" /> level.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The custom error message. If null, the exception's message is used.</param>
        public void Error(Exception exception, string? message = null)
        {
            // If message is null, it defaults to exception.Message to ensure the log entry has context.
            logger.LogError(exception, message ?? exception.Message);
        }
    }
}