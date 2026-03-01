namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Defines a specific task or operation that can be performed on the dropped payload.
/// </summary>
public sealed record DropAction(
    string Id, // Unique identifier for the action.
    string Title, // Human-readable name shown in the UI.
    int Priority, // Order of appearance (higher usually means more prominent).
    Func<bool>? CanExecute, // Optional logic to determine if the action is valid.
    Func<IProgress<double>?, Task> ExecuteAsync, // The core logic to run the action with progress reporting.
    string? IconKey = null, // Key for looking up an associated icon/glyph.
    bool IsDefault = false, // Whether this is the "primary" action for a double-click/default drop.
    bool Localization = true) // Flag to indicate if the title should be localized.
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