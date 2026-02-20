using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;

namespace ZYC.Framework.Modules.Log;

internal sealed class Log4NetLoggerProvider : ILoggerProvider
{
    public Log4NetLoggerProvider(string configFile)
    {
        XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(configFile));
    }

    public ILogger CreateLogger(string categoryName)
        => new Log4NetLogger(LogManager.GetLogger(categoryName));

    public void Dispose() { }
}