using System.ComponentModel;
using System.Windows.Interop;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions;
using WindowState = System.Windows.WindowState;

namespace ZYC.Framework.MetroWindow.UI;

internal partial class MainWindow : IMainWindow
{
    public void SetTopmost(bool topmost)
    {
        Topmost = topmost;
        DesktopWindowState.Topmost = topmost;
    }

    public void SetWindowState(Abstractions.WindowState windowState)
    {
        WindowState = windowState.Cast<WindowState>();
    }

    public Abstractions.WindowState GetWindowState()
    {
        return WindowState.Cast<Abstractions.WindowState>();
    }

    public void InitContent(object content)
    {
        DebuggerTools.CheckCalledOnce();
        Content = content;
    }


    public void SetWindowWidth(int width)
    {
        Width = width;
        DesktopWindowState.Width = width;
    }

    public void SetWindowHeight(int height)
    {
        Height = height;
        DesktopWindowState.Height = height;
    }

    public bool GetShowInTaskbar()
    {
        return ShowInTaskbar;
    }

    public IntPtr GetWindowHandle()
    {
        var windowHandle = new WindowInteropHelper(this).EnsureHandle();
        return windowHandle;
    }

    public void SetIsFrozen(bool value)
    {
        IsEnabled = !value;
        DesktopWindowState.IsFrozen = value;
    }

    public object GetMainWindow()
    {
        return this;
    }

    public void SetShowInTaskbar(bool value)
    {
        ShowInTaskbar = value;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        //!WARNING Centralize the exit code into AppContext
        e.Cancel = true;

        var appContext = LifetimeScope.Resolve<IAppContext>();
        appContext.ExitProcess();
    }

    private void OnAppContextExiting(object? sender, CancelEventArgsEx e)
    {
        if (e.Handled)
        {
            return;
        }

        if (DesktopWindowState.IsPreventExit)
        {
            e.Cancel = true;
        }
    }
}