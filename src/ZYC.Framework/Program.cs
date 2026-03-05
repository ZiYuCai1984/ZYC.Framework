using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Windows;
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
    private static void StartApp()
    {
        //TODO-zyc The design is flawed and needs to be modified to support switching between multiple versions.
        var mainAppFolder = AppContext.GetMainAppDirectory();

        var appState = SettingsTools.GetFromFolder<AppState>(
            mainAppFolder);

        if (appState.StartupTarget == StartupTarget.Main)
        {
            if (AppContext.IsSelfAlternate())
            {
                RestartToMainApp();
            }
            else
            {
                return;
            }
        }
        else if (appState.StartupTarget == StartupTarget.Alternate)
        {
            if (!AppContext.IsSelfAlternate())
            {
                RestartToAlternateApp();
            }
            else
            {
                return;
            }
        }

        throw new InvalidOperationException($"Unknown {nameof(StartupTarget)}:{appState.StartupTarget}");
    }


    private static void RestartToMainApp()
    {
        var fileName = AppContext.GetProcessFileName();

        var mainAppFolder = AppContext.GetMainAppDirectory();
        var mainAppPath = Path.Combine(mainAppFolder, fileName);

        Process.Start(
            new ProcessStartInfo(mainAppPath,
                AppContext.GetArgumentString())
            {
                WorkingDirectory = mainAppFolder
            });

        AppContext.FocusExitProcess();
    }

    private static void RestartToAlternateApp()
    {
        var fileName = AppContext.GetProcessFileName();
        var folder = AppContext.GetAlternateAppDirectory();

        var alternateAppPath = Path.Combine(folder,
            fileName);

        Process.Start(new ProcessStartInfo(
            alternateAppPath,
            AppContext.GetArgumentString())
        {
            WorkingDirectory = folder
        });

        AppContext.FocusExitProcess();
    }

    [STAThread]
    private static void Main()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

#if DEBUG

#else
        EnsureSingleInstance();
#endif

        InitJsonToolsSettings();


        StartApp();

        var builder = new ContainerBuilder();

        builder.RegisterGeneric(typeof(Infrastructure.NullLogger<>))
            .As(typeof(IAppLogger<>));


        builder.RegisterInstance(NullLoggerFactory.Instance)
            .As<ILoggerFactory>()
            .SingleInstance();

        builder.RegisterGeneric(typeof(Logger<>))
            .As(typeof(ILogger<>));


        var appContextDirectory = AppContext.GetMainAppDirectory();


        ModuleTools.RegisterAllFromAssembly(appContextDirectory,
            builder,
            typeof(Program).Assembly);
        ModuleTools.RegisterAllFromAssembly(appContextDirectory,
            builder,
            AssemblyInfo.GetAssembly());
        ModuleTools.RegisterAllFromAssembly(appContextDirectory,
            builder,
            typeof(WebViewHostBase).Assembly);

        ModuleTools.RegisterAllFromAssembly(appContextDirectory,
            builder,
            MetroWindow.AssemblyInfo.GetAssembly());

        ModuleConfig? moduleConfig = null;
        PendingFileOperationsState? pendingDeleteState = null;

        ModuleTools.RegisterAllFromAssembly(appContextDirectory,
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
            appContextDirectory,
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
        ReactiveExtensions.SetSynchronizationContext(app.GetUISynchronizationContext());

        container.RegisterTabItemFactory<ModuleLoadTabItemFactory>();
        container.Resolve<IModuleLoadInfoManager>().
            SetLoadErrorInfos(moduleLoadErrorInfoList.ToArray());

        var mainWindowView = container.Resolve<MainWindowView>();
        var mainWindow = container.Resolve<IMainWindow>();
        mainWindow.InitContent(mainWindowView);

        var window = (Window)mainWindow;
        app.Run(window);
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