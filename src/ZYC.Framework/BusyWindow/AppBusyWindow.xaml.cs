using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.BusyWindow;

namespace ZYC.Framework.BusyWindow;

[RegisterAs(typeof(IAppBusyWindow))]
internal partial class AppBusyWindow : IAppBusyWindow
{
    public static readonly DependencyProperty ShowProgressProperty =
        DependencyProperty.Register(
            nameof(ShowProgress),
            typeof(bool),
            typeof(AppBusyWindow),
            new PropertyMetadata(true));

    public static readonly DependencyProperty StatusTextProperty =
        DependencyProperty.Register(
            nameof(StatusText),
            typeof(string),
            typeof(AppBusyWindow),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsProgressIndeterminateProperty =
        DependencyProperty.Register(
            nameof(IsProgressIndeterminate),
            typeof(bool),
            typeof(AppBusyWindow),
            new PropertyMetadata(true));

    public static readonly DependencyProperty ProgressValueProperty =
        DependencyProperty.Register(
            nameof(ProgressValue),
            typeof(double),
            typeof(AppBusyWindow),
            new PropertyMetadata(0.0));


    public AppBusyWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public static string DefaultTitle => "Loading";

    private object HandlerSyncRoot { get; } = new();

    private LinkedList<BusyWindowHandler> HandlerQueue { get; } = new();

    private BusyWindowHandler? CurrentHandler { get; set; }

    public bool IsProgressIndeterminate
    {
        get => (bool)GetValue(IsProgressIndeterminateProperty);
        set => SetValue(IsProgressIndeterminateProperty, value);
    }

    public double ProgressValue
    {
        get => (double)GetValue(ProgressValueProperty);
        set => SetValue(ProgressValueProperty, value);
    }

    public bool ShowProgress
    {
        get => (bool)GetValue(ShowProgressProperty);
        set => SetValue(ShowProgressProperty, value);
    }


    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public IBusyWindowHandler Enqueue()
    {
        var handler = new BusyWindowHandler(this);

        lock (HandlerSyncRoot)
        {
            if (CurrentHandler == null)
            {
                Debug.Assert(!IsVisible);

                CurrentHandler = handler;
                handler.Activate();

                Debug.Assert(Dispatcher != null);
                Dispatcher.Invoke(Show);
            }
            else
            {
                HandlerQueue.AddLast(handler);
            }
        }

        return handler;
    }

    internal void Close(BusyWindowHandler handler)
    {
        lock (HandlerSyncRoot)
        {
            if (handler == CurrentHandler)
            {
                CurrentHandler = null;

                if (HandlerQueue.Count == 0)
                {
                    Debug.Assert(Dispatcher != null);
                    Dispatcher.Invoke(Hide);
                    return;
                }

                CurrentHandler = HandlerQueue.First!.Value;
                HandlerQueue.RemoveFirst();
                CurrentHandler.Activate();
            }
            else
            {
                HandlerQueue.Remove(handler);
            }

            handler.Deactivate();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Hide close button
        var hwnd = new WindowInteropHelper(this).Handle;
        Win32API.SetWindowLong(
            hwnd,
            Win32API.GWL_STYLE,
            Win32API.GetWindowLong(hwnd, Win32API.GWL_STYLE) & ~Win32API.WS_SYSMENU);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    private static class Win32API
    {
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}