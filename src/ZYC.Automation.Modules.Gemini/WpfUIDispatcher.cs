using System.Windows.Threading;

namespace ZYC.Automation.Modules.Gemini;

public sealed class WpfUIDispatcher : IUIDispatcher
{
    private readonly Dispatcher _dispatcher;

    public WpfUIDispatcher(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
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