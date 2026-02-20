using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Configures task manager behavior.
/// </summary>
public sealed class TaskManagerConfig : IConfig
{
    /// <summary>
    ///     Gets the maximum concurrency.
    /// </summary>
    public int MaxConcurrency { get; init; } = 5;

    /// <summary>
    ///     Gets a value indicating whether queued tasks auto-start when loading.
    /// </summary>
    public bool AutoStartQueuedOnLoad { get; init; } = true;

    /// <summary>
    ///     Gets a value indicating whether tasks auto-start when enqueued.
    /// </summary>
    public bool AutoStartOnEnqueue { get; init; } = true;
}