namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Defines a specific task or operation that can be performed on the dropped payload.
/// </summary>
/// <param name="Id">Unique identifier for the action.</param>
/// <param name="Title">Human-readable name shown in the UI.</param>
/// <param name="Priority">Order of appearance (higher usually means more prominent).</param>
/// <param name="CanExecute">Optional logic to determine if the action is valid.</param>
/// <param name="ExecuteAsync">The core logic to run the action with progress reporting.</param>
/// <param name="IconKey">Key for looking up an associated icon/glyph.</param>
/// <param name="IsDefault">Whether this is the "primary" action for a double-click/default drop.</param>
/// <param name="Localization">Flag to indicate if the title should be localized.</param>
public sealed record DropAction(
    string Id,
    string Title,
    int Priority,
    Func<bool>? CanExecute,
    Func<IProgress<double>?, Task> ExecuteAsync,
    string? IconKey = null,
    bool IsDefault = false,
    bool Localization = true)
{
    /// <summary>
    ///     Evaluates whether the action can be run based on the current state.
    /// </summary>
    public bool CanRun()
    {
        return CanExecute?.Invoke() ?? true;
    }

    /// <summary>
    ///     Triggers the execution of the action.
    /// </summary>
    public Task RunAsync(IProgress<double>? progress = null)
    {
        return ExecuteAsync(progress);
    }
}