using System.Windows.Threading;
using ZYC.Automation.Modules.MCP.Server.Abstractions;

namespace ZYC.Automation.Modules.MCP.Server;

public sealed class UIDispatcher : IUIDispatcher
{
    private readonly Dispatcher _dispatcher;

    public UIDispatcher(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

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