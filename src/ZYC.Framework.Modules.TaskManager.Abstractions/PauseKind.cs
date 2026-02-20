namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Defines the pause mode for a managed task.
/// </summary>
public enum PauseKind
{
    /// <summary>
    ///     Not paused.
    /// </summary>
    None,

    /// <summary>
    ///     Paused while queued.
    /// </summary>
    PausedWhileQueuing,

    /// <summary>
    ///     Paused while running.
    /// </summary>
    PausedWhileRunning
}