namespace ZYC.Automation.Modules.Gemini;

public interface IUIDispatcher
{
    Task InvokeAsync(Action action);
    Task<T> InvokeAsync<T>(Func<T> func);
    Task InvokeAsync(Func<Task> func);
    Task<T> InvokeAsync<T>(Func<Task<T>> func);
}