using System.IO;
using Autofac;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging.Abstractions;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Log;

internal class FooLogger<T> : IAppLogger<T>
{
    private ILog? _log;

    public FooLogger(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    private ILog Log
    {
        get
        {
            if (_log == null)
            {
                _log = LogManager.GetLogger(nameof(FooLogger<T>));

                var app = LifetimeScope.Resolve<IAppContext>();
                var fileName = $"{app.GetMainAppDirectory()}\\log4net.config";

                XmlConfigurator.Configure(new FileInfo(fileName));
            }

            return _log;
        }
    }

    public void Error(string message)
    {
        Log.Error(message);
    }

    public void Info(string message)
    {
        Log.Info(message);
    }

    public void Debug(string message)
    {
#if DEBUG
        Log.Debug(message);
#endif
    }

    public void Error(Exception exception)
    {
        Log.Error(exception);
    }

    public void Warn(string message)
    {
        Log.Warn(message);
    }

    public object GetMicrosoftExtensionsLogger()
    {
        return NullLogger.Instance;
    }
}