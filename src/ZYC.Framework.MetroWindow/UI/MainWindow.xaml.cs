using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using Autofac;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Core;
using WindowState = System.Windows.WindowState;

namespace ZYC.Framework.MetroWindow.UI;

[RegisterSingleInstanceAs(typeof(IMainWindow), typeof(MainWindow), typeof(IDialogManager))]
internal partial class MainWindow
{
    public static readonly DependencyProperty LifetimeScopeProperty = DependencyProperty.Register(
        nameof(LifetimeScope),
        typeof(ILifetimeScope),
        typeof(MainWindow),
        new PropertyMetadata(null, OnLifetimeScopeChanged)
    );

    public MainWindow(
        ILogger<MainWindow> logger,
        AppConfig appConfig,
        ILifetimeScope lifetimeScope,
        DesktopWindowState desktopWindowState)
    {
        Logger = logger;
        AppConfig = appConfig;
        DesktopWindowState = desktopWindowState;

        InitializeComponent();

        LifetimeScope = lifetimeScope;

        SetMenuDropAlignmentRight();

        appConfig.ObserveProperty(nameof(AppConfig.ShowInTaskbar))
            .Throttle(TimeSpan.FromMilliseconds(200))
            .ObserveOnUI()
            .Subscribe(_ =>
            {
                SetShowInTaskbar(appConfig.ShowInTaskbar);
            }).DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();


    /// <summary>
    ///     !WARNING There is a bug in using the converter, so this is how it is implemented temporarily
    /// </summary>
    public WindowState WindowStateValue
    {
        get => DesktopWindowState.WindowState.Cast<WindowState>();
        set => DesktopWindowState.WindowState = value.Cast<Abstractions.WindowState>();
    }

    /// <summary>
    ///     !WARNING Suppress updating when maximizing
    /// </summary>
    public int DesktopWindowStateWidthValue
    {
        get => DesktopWindowState.Width;
        set
        {
            if (DesktopWindowState.WindowState == Abstractions.WindowState.Maximized)
            {
                return;
            }

            DesktopWindowState.Width = value;
        }
    }

    /// <summary>
    ///     !WARNING Suppress updating when maximizing
    /// </summary>
    public int DesktopWindowStateHeightValue
    {
        get => DesktopWindowState.Height;
        set
        {
            if (DesktopWindowState.WindowState == Abstractions.WindowState.Maximized)
            {
                return;
            }

            DesktopWindowState.Height = value;
        }
    }


    private ILogger<MainWindow> Logger { get; }
    public AppConfig AppConfig { get; }

    public DesktopWindowState DesktopWindowState { get; }

    public ILifetimeScope LifetimeScope
    {
        get => (ILifetimeScope)GetValue(LifetimeScopeProperty);
        private set => SetValue(LifetimeScopeProperty, value);
    }

    private static void OnLifetimeScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var component = (MainWindow)d;
        component.LifetimeScope = (ILifetimeScope)e.NewValue;
    }

    private IntPtr ActivateWindowHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == NativeMethods.WM_SHOWME)
        {
            handled = true;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (WindowState == WindowState.Minimized)
                {
                    WindowState = WindowState.Normal;
                }

                Show();
                Activate();
            }));
        }

        return IntPtr.Zero;
    }


    private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
    {
        DebuggerTools.CheckCalledOnce();


        var hwndSource = (HwndSource)PresentationSource.FromVisual(this)!;
        hwndSource.AddHook(ActivateWindowHook);

        var appContext = LifetimeScope.Resolve<IAppContext>();
        appContext.Exiting += OnAppContextExiting;
    }

    public void SetMenuDropAlignmentRight()
    {
        try
        {
            var ifLeft = SystemParameters.MenuDropAlignment;
            if (!ifLeft)
            {
                return;
            }

            var t = typeof(SystemParameters);
            var field = t.GetField("_menuDropAlignment",
                BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, false);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}