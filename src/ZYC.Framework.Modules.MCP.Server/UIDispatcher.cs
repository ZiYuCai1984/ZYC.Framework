using System.Windows;
using System.Windows.Threading;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server;

[RegisterSingleInstanceAs(typeof(IUIDispatcher))]
public sealed class UIDispatcher : IUIDispatcher
{
    private readonly Dispatcher _dispatcher = Application.Current.Dispatcher;

    public bool CheckAccess()
    {
        return _dispatcher.CheckAccess();
    }

    public Task InvokeAsync(Action action)
    {
        return _dispatcher.InvokeAsync(action).Task;
    }

    public Task<T> InvokeAsync<T>(Func<T> func)
    {
        return _dispatcher.InvokeAsync(func).Task;
    }

    public Task InvokeAsync(Func<Task> func)
    {
        return _dispatcher.InvokeAsync(func).Task.Unwrap();
    }

    public Task<T> InvokeAsync<T>(Func<Task<T>> func)
    {
        return _dispatcher.InvokeAsync(func).Task.Unwrap();
    }
}