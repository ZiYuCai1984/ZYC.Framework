using Microsoft.Extensions.Logging;

namespace ZYC.Framework.Abstractions;

public static class AppLoggerCompatExtensions
{
    extension(ILogger logger)
    {
        public void Info(string message)
        {
            logger.LogInformation(message);
        }

        public void Debug(string message)
        {
            logger.LogDebug(message);
        }

        public void Warn(string message)
        {
            logger.LogWarning(message);
        }

        public void Error(string message)
        {
            logger.LogError(message);
        }

        public void Error(Exception exception, string? message = null)
        {
            logger.LogError(exception, message ?? exception.Message);
        }
    }
}