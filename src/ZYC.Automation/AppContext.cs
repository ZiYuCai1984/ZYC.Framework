using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Abstractions.Config;
using ZYC.Automation.Abstractions.State;
using ZYC.Automation.Core;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.CoreToolkit.Extensions.Settings;

namespace ZYC.Automation;

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

    string IAppContext.GetMainAppDirectory()
    {
        return GetMainAppDirectory();
    }

    string IAppContext.GetAlternateAppDirectory()
    {
        return GetAlternateAppDirectory();
    }

    string IAppContext.GetProcessFileName()
    {
        return GetProcessFileName();
    }

    public string GetDefaultWebView2UserDataFolder()
    {
        return Path.Combine(GetMainAppDirectory(), $"{GetProcessFileName()}.WebView2");
    }

    bool IAppContext.IsSelfAlternate()
    {
        return IsSelfAlternate();
    }


    public void SaveAllConfig()
    {
        var mainAppFolder = GetMainAppDirectory();

        var configs = LifetimeScope.Resolve<IConfig[]>();
        foreach (var config in configs)
        {
            SettingsTools.SetToFolderGeneric(mainAppFolder, config);
        }
    }

    public void SaveAllState()
    {
        var mainAppFolder = GetMainAppDirectory();

        var states = LifetimeScope.Resolve<IState[]>();
        foreach (var state in states)
        {
            SettingsTools.SetToFolderGeneric(mainAppFolder, state);
        }
    }

    public void SwitchStartupTarget()
    {
        StartupTarget target;

        if (!IsSelfAlternate())
        {
            target = StartupTarget.Alternate;
        }
        else
        {
            target = StartupTarget.Main;
        }

        AppState.StartupTarget = target;
        Logger.Warn($"Switch startup target -> {target.ToString()}");
    }

    string IAppContext.GetArgumentString()
    {
        return GetArgumentString();
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


    public static string GetArgumentString()
    {
        var arguments = Environment.GetCommandLineArgs();
        var argumentString = string.Join(" ", arguments.Skip(1));
        return argumentString;
    }

    public static string GetAlternateAppDirectory()
    {
        var main = GetMainAppDirectory();
        return Path.Combine(main, AlternateFolderName);
    }

    public static string GetCurrentDirectory()
    {
        return IOTools.GetExecutingFolder();
    }

    public static string GetMainAppDirectory()
    {
        var current = GetCurrentDirectory();

        var directory = new DirectoryInfo(current);
        if (directory.Name != AlternateFolderName)
        {
            return directory.FullName;
        }

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
}