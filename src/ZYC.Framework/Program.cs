using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Windows;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.CoreToolkit.Extensions.Settings;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.CLI;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Localizations;
using ZYC.Framework.Tab.BuildIn;
using ZYC.Framework.WebView2;
using AssemblyInfo = ZYC.Framework.Core.AssemblyInfo;
using ModuleNameTools = ZYC.CoreToolkit.Extensions.Settings.ModuleNameTools;

namespace ZYC.Framework;

#pragma warning disable CA1416

internal partial class Program
{
    static Program()
    {
        System.AppContext.SetSwitch(
            "Switch.System.Windows.Appearance.DisableFluentThemeWindowBackdrop",
            true);
    }


    private static void RedirectToStartupVersion()
    {
        var settingsDirectory = AppContext.GetSettingsDirectory();

        var appState = SettingsTools.GetFromFolder<AppState>(
            settingsDirectory);

        if (appState.StartupVersion == ProductInfo.Version)
        {
        }
        else
        {
            RestartToSpecifiedVersion(appState.StartupVersion);
        }
    }


    private static void RestartToSpecifiedVersion(string version)
    {
        var fileName = AppContext.GetProcessFileName();

        var appRootDirectory = AppContext.GetAppRootDirectory();
        var targetAppFolder = Path.Combine(appRootDirectory, version);
        var targetAppFile = Path.Combine(appRootDirectory, version, fileName);

        if (IOTools.FileExists(targetAppFile))
        {
            Process.Start(
                new ProcessStartInfo(targetAppFile,
                    AppContext.GetArgumentString())
                {
                    WorkingDirectory = targetAppFolder
                });

            AppContext.FocusExitProcess();
        }
        else
        {
            MessageBoxTools.Warning(
                $"Cannot start version '{version}'.\n\n" +
                $"Executable not found:\n{targetAppFile}", "Warning", false);
        }
    }


    [STAThread]
    private static void Main()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        var startupUri = StartupUriParser.GetStartupUriArgument();

#if DEBUG

#else
        EnsureSingleInstance(startupUri);
#endif

        InitJsonToolsSettings();

#if DEBUG

#else
        RedirectToStartupVersion();
#endif

        var startupUriNavigationDispatcher = new StartupUriNavigationDispatcher();
#if DEBUG
        StartupUriPipeServer? startupUriPipeServer = null;
#else
        var startupUriPipeServer = StartStartupUriPipeServer(startupUriNavigationDispatcher);
#endif

        var builder = new ContainerBuilder();

        builder.RegisterGeneric(typeof(Infrastructure.NullLogger<>))
            .As(typeof(IAppLogger<>));


        builder.RegisterInstance(NullLoggerFactory.Instance)
            .As<ILoggerFactory>()
            .SingleInstance();

        builder.RegisterGeneric(typeof(Logger<>))
            .As(typeof(ILogger<>));


        var settingsDirectory = AppContext.GetSettingsDirectory();


        ModuleTools.RegisterAllFromAssembly(settingsDirectory,
            builder,
            typeof(Program).Assembly);
        ModuleTools.RegisterAllFromAssembly(settingsDirectory,
            builder,
            AssemblyInfo.GetAssembly());
        ModuleTools.RegisterAllFromAssembly(settingsDirectory,
            builder,
            typeof(WebViewHostBase).Assembly);

        ModuleTools.RegisterAllFromAssembly(settingsDirectory,
            builder,
            MetroWindow.AssemblyInfo.GetAssembly());

        ModuleConfig? moduleConfig = null;
        PendingFileOperationsState? pendingDeleteState = null;


        ModuleTools.RegisterAllFromAssembly(settingsDirectory,
            builder,
            typeof(ProductInfo).Assembly, obj =>
            {
                if (obj is ModuleConfig initModuleConfig)
                {
                    moduleConfig = initModuleConfig;
                }

                if (obj is PendingFileOperationsState initPendingFileOperationsState)
                {
                    pendingDeleteState = initPendingFileOperationsState;
                }
            });

        Debug.Assert(moduleConfig != null);
        Debug.Assert(pendingDeleteState != null);

        var startupLogger = StartupLogger.CreateInstance();
        var modules = ModuleTools.RegisterModules(
            settingsDirectory,
            builder,
            moduleConfig,
            pendingDeleteState,
            startupLogger);


        var moduleLoadErrorInfoList = new List<ModuleLoadErrorInfo>();


        var container = builder.Build();
        foreach (var module in modules)
        {
            if (!module.IsEnabled)
            {
                continue;
            }

            try
            {
                module.LoadAsync(container).Wait();
            }
            catch (Exception e)
            {
                moduleLoadErrorInfoList.Add(
                    new ModuleLoadErrorInfo(module,
                        e,
                        nameof(ModuleBase.LoadAsync)));
            }
        }

        foreach (var module in modules)
        {
            if (!module.IsEnabled)
            {
                continue;
            }

            try
            {
                module.AfterLoadedAsync(container).Wait();
            }
            catch (Exception e)
            {
                moduleLoadErrorInfoList.Add(
                    new ModuleLoadErrorInfo(module,
                        e,
                        nameof(ModuleBase.AfterLoadedAsync)));
            }
        }


        L.SetLifetimeScope(container);

        var app = container.Resolve<AppContext>();
        app.Exit += (_, _) =>
        {
            startupUriPipeServer?.Dispose();
            startupUriNavigationDispatcher.Dispose();
        };

        ReactiveExtensions.SetSynchronizationContext(app.GetUISynchronizationContext());

        container.RegisterTabItemFactory<ModuleLoadTabItemFactory>();
        container.Resolve<IModuleLoadInfoManager>().SetLoadErrorInfos(moduleLoadErrorInfoList.ToArray());

        var mainWindowView = container.Resolve<MainWindowView>();
        var mainWindow = container.Resolve<IMainWindow>();
        mainWindow.InitContent(mainWindowView);

        RegisterStartupUriNavigation(container, startupUriNavigationDispatcher, startupUri);

        var window = (Window)mainWindow;
        app.Run(window);
    }

    private static StartupUriPipeServer? StartStartupUriPipeServer(
        StartupUriNavigationDispatcher startupUriNavigationDispatcher)
    {
        var pipeName = GetStartupUriPipeName();
        if (string.IsNullOrWhiteSpace(pipeName))
        {
            return null;
        }

        var server = new StartupUriPipeServer(pipeName, startupUriNavigationDispatcher.Enqueue);
        server.Start();
        return server;
    }

    private static void RegisterStartupUriNavigation(
        ILifetimeScope container,
        StartupUriNavigationDispatcher startupUriNavigationDispatcher,
        Uri? startupUri)
    {
        startupUriNavigationDispatcher.Register(container, startupUri);
    }

    private static void InitJsonToolsSettings()
    {
        ModuleNameTools.SetModulePrefix("ZYC.Framework.Modules.");
        JsonTools.SetDefaultJsonSerializerOptions(new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            Converters = { new JsonStringEnumConverter() }
        });
    }
}
