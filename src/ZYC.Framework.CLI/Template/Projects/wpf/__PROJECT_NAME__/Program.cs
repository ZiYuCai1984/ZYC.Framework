using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.CoreToolkit.Extensions.Settings;

namespace __PROJECT_NAME__;

internal class Program
{
    [STAThread]
    private static void Main()
    {
        var containerBuilder = new ContainerBuilder();

        RegisterAllFromAssembly(
            Path.GetDirectoryName(typeof(Program).Assembly.Location)!,
            containerBuilder,
            typeof(Program).Assembly);

        var container = containerBuilder.Build();

        var app = container.Resolve<App>();
        var mainWindow = container.Resolve<MainWindow>();

        app.Run(mainWindow);
    }


    public static void RegisterAllFromAssembly(
        string settingsDirectory,
        ContainerBuilder builder,
        Assembly assembly,
        Action<object>? registerAction = null)
    {
        var folder = settingsDirectory;

        Trace.WriteLine($"Register from {assembly.FullName}");
        AutofacTools.RegisterFromAssembly(builder, assembly);

        var types = assembly.SafeGetTypes();
        foreach (var type in types)
        {
            if (typeof(IConfig).IsAssignableFrom(type)
                && type != typeof(IConfig)
                && !type.IsInterface
                && !type.IsAbstract)
            {
                var result = SettingsTools.GetFromFolderGeneric(folder, type);
                builder.RegisterConfigR(result);

                registerAction?.Invoke(result);

                continue;
            }

            if (typeof(IState).IsAssignableFrom(type)
                && type != typeof(IState)
                && !type.IsInterface
                && !type.IsAbstract)
            {
                var result = SettingsTools.GetFromFolderGeneric(folder, type);
                builder.RegisterStateR(result);

                registerAction?.Invoke(result);

                // ReSharper disable once RedundantJumpStatement
                continue;
            }
        }
    }
}