namespace ZYC.Framework.Abstractions.DragDrop;

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
    public bool CanRun()
    {
        return CanExecute?.Invoke() ?? true;
    }

    public Task RunAsync(IProgress<double>? progress = null)
    {
        return ExecuteAsync(progress);
    }
}