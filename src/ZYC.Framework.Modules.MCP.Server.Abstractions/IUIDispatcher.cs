namespace ZYC.Framework.Modules.MCP.Server.Abstractions;

/// <summary>
///     Provides a mechanism to dispatch operations to the UI (Main) thread.
/// </summary>
public interface IUIDispatcher
{
    /// <summary>
    ///     Determines whether the calling thread is the UI thread.
    /// </summary>
    /// <returns>True if the caller can access the UI directly; otherwise, false.</returns>
    bool CheckAccess();

    /// <summary>
    ///     Executes the specified action asynchronously on the UI thread.
    /// </summary>
    Task InvokeAsync(Action action);

    /// <summary>
    ///     Executes the specified function asynchronously on the UI thread and returns the result.
    /// </summary>
    Task<T> InvokeAsync<T>(Func<T> func);

    /// <summary>
    ///     Executes the specified asynchronous function on the UI thread.
    /// </summary>
    Task InvokeAsync(Func<Task> func);

    /// <summary>
    ///     Executes the specified asynchronous function on the UI thread and returns the task result.
    /// </summary>
    Task<T> InvokeAsync<T>(Func<Task<T>> func);
}