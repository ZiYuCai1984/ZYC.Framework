using Autofac;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.CoreToolkit.Extensions.Settings;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Core;

namespace ZYC.Framework;

[RegisterSingleInstanceAs(typeof(AppContext), typeof(IAppContext))]
internal partial class AppContext : IAppContext
{
    public AppContext(
        AppState appState,
        ILifetimeScope lifetimeScope,
        ModuleBase[] modules,
        IAppLogger<AppContext> logger,
        AppConfig appConfig)
    {
        InitializeComponent();

        AppState = appState;
        LifetimeScope = lifetimeScope;
        Modules = modules;
        Logger = logger;
        AppConfig = appConfig;

        DispatcherUnhandledException += OnAppDispatcherUnhandledException;

        AppDomain.CurrentDomain.UnhandledException += OnAppDomainExceptionUnhandled;
        TaskScheduler.UnobservedTaskException += OnTaskExceptionUnhandled;


        appConfig.ObserveProperty(nameof(Abstractions.Config.AppConfig.StartAtBoot))
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Subscribe(_ =>
            {
                try
                {
                    if (appConfig.StartAtBoot)
                    {
                        ShortcutTools.AddToStartupFolder();
                    }
                    else
                    {
                        ShortcutTools.RemoveFromStartupFolder();
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            });


        appConfig.ObserveProperty(nameof(Abstractions.Config.AppConfig.DesktopShortcut))
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Subscribe(_ =>
            {
                try
                {
                    if (appConfig.DesktopShortcut)
                    {
                        ShortcutTools.CreateFromCurrentProcess();
                    }
                    else
                    {
                        var fileNameWithoutExe = IOTools.GetFileName(
                            GetProcessFileName(),
                            false);

                        ShortcutTools.Delete($"{fileNameWithoutExe}.lnk");
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            });
    }

    private static string AlternateFolderName => "Alternate";

    private AppState AppState { get; }

    private ILifetimeScope LifetimeScope { get; }

    private ModuleBase[] Modules { get; }

    private IAppLogger<AppContext> Logger { get; }

    private AppConfig AppConfig { get; }

    private static SynchronizationContext? UISynchronizationContext { get; set; }

    string IAppContext.GetCurrentDirectory()
    {
        return GetCurrentDirectory();
    }

    public string GetTempPath()
    {
        return Path.GetTempPath();
    }

    string IAppContext.GetProcessFileName()
    {
        return GetProcessFileName();
    }


    public void SaveAllConfig()
    {
        var settingsFolder = GetSettingsDirectory();

        var configs = LifetimeScope.Resolve<IConfig[]>();
        foreach (var config in configs)
        {
            SettingsTools.SetToFolderGeneric(settingsFolder, config);
        }
    }

    public void SaveAllState()
    {
        var settingsFolder = GetSettingsDirectory();

        var states = LifetimeScope.Resolve<IState[]>();
        foreach (var state in states)
        {
            SettingsTools.SetToFolderGeneric(settingsFolder, state);
        }
    }

    string IAppContext.GetArgumentString()
    {
        return GetArgumentString();
    }

    public void UpdateStartupVersion(string version)
    {
        AppState.StartupVersion = version;
        Logger.Warn($"Switch startup version -> {version}");

        SettingsTools.SetToFolderGeneric(GetSettingsDirectory(), AppState);
    }

    public SynchronizationContext GetUISynchronizationContext()
    {
        if (UISynchronizationContext != null)
        {
            return UISynchronizationContext;
        }

        InvokeOnUIThread(() =>
        {
            UISynchronizationContext = SynchronizationContext.Current
                                       ?? new DispatcherSynchronizationContext();
        });

        return UISynchronizationContext!;
    }

    public void InvokeOnUIThread(Action action)
    {
        Dispatcher.Invoke(action);
    }

    public async Task InvokeOnUIThreadAsync(Func<Task> func)
    {
        var taskCompletionSource = new TaskCompletionSource();

        Dispatcher.Invoke(() =>
        {
            func.Invoke().ContinueWith(_ =>
            {
                taskCompletionSource.SetResult();
            });
        });

        await taskCompletionSource.Task;
    }

    public async Task<T> InvokeOnUIThreadAsync<T>(Func<Task<T>> func)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();

        Dispatcher.Invoke(() =>
        {
            func.Invoke().ContinueWith(t =>
            {
                taskCompletionSource.SetResult(t.Result);
            });
        });

        return await taskCompletionSource.Task;
    }

    string IAppContext.GetSettingsDirectory()
    {
        return GetSettingsDirectory();
    }

    public string GetDefaultWebView2UserDataFolder()
    {
        return Path.Combine(GetSettingsDirectory(), $"{GetProcessFileName()}.WebView2");
    }

    public static string GetArgumentString()
    {
        var arguments = Environment.GetCommandLineArgs();
        var argumentString = string.Join(" ", arguments.Skip(1));
        return argumentString;
    }

    public static string GetCurrentDirectory()
    {
        return IOTools.GetExecutingFolder();
    }


    public static string GetSettingsDirectory()
    {
        var current = GetCurrentDirectory();

        var directory = new DirectoryInfo(current);
        return directory.Parent!.FullName;
    }

    public static string GetProcessFileName()
    {
        var fullFileName = Process.GetCurrentProcess().MainModule!.FileName;
        return Path.GetFileName(fullFileName);
    }

    public static bool IsSelfAlternate()
    {
        var dir = Path.GetFileName(GetCurrentDirectory());
        var result = dir == AlternateFolderName;

        return result;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        base.OnStartup(e);
    }

    private Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var message = $"Resolve <{args.Name}>";

        Logger.Info(message);


#if DEBUG
        Trace.WriteLine(message);
#endif


        return null;
    }

    public static string GetMainAppDirectory()
    {
        throw new NotImplementedException();
    }
}