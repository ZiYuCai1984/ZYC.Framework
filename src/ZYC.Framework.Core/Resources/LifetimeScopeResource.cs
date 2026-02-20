using System.Diagnostics;
using System.Windows;
using Autofac;

namespace ZYC.Framework.Core.Resources;

public class LifetimeScopeResource
{
    private static ILifetimeScope? _lifetimeScope;

    public ILifetimeScope LifetimeScope => GetRootLifetimeScopeFromMainWindowDataContext();

    public static ILifetimeScope GetRootLifetimeScopeFromMainWindowDataContext()
    {
        if (_lifetimeScope != null)
        {
            return _lifetimeScope;
        }

        var window = Application.Current.MainWindow;

        Debug.Assert(window != null);
        if (window == null)
        {
            throw new InvalidOperationException("MainWindow is not initialized yet.");
        }


        _lifetimeScope = (ILifetimeScope)window.DataContext;
        return _lifetimeScope;
    }
}