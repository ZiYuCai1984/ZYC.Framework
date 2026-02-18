using Autofac;
using Microsoft.Extensions.Logging;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Log.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Automation.Modules.Log;

internal class Module : ModuleBase
{
    public override string Icon => LogModuleConstants.Icon;

    public override Task RegisterAsync(ContainerBuilder builder)
    {
        builder.RegisterType<Log4NetLoggerProvider>()
            .WithParameter("configFile", "log4net.config")
            .As<ILoggerProvider>()
            .SingleInstance();

        builder.Register(ctx =>
            {
                var providers = ctx.Resolve<IEnumerable<ILoggerProvider>>();
                var factory = LoggerFactory.Create(b =>
                {
                    b.ClearProviders();
                    foreach (var p in providers) b.AddProvider(p);
                });
                return factory;
            })
            .As<ILoggerFactory>()
            .SingleInstance();

        builder.RegisterGeneric(typeof(FooLogger<>)).As(typeof(IAppLogger<>));

        return base.RegisterAsync(builder);
    }

    public override async Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterFileMainMenuItem<LogMainMenuItem>();
        await Task.CompletedTask;
    }
}