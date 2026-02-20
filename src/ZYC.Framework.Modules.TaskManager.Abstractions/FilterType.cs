namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Defines task list filter options.
/// </summary>
public enum FilterType
{
    /// <summary>
    ///     All tasks.
    /// </summary>
    All,

    /// <summary>
    ///     Running tasks.
    /// </summary>
    Running,

    /// <summary>
    ///     Paused tasks.
    /// </summary>
    Paused,

    /// <summary>
    ///     Completed tasks.
    /// </summary>
    Completed,

    /// <summary>
    ///     Faulted tasks.
    /// </summary>
    Faulted,

    /// <summary>
    ///     Canceled tasks.
    /// </summary>
    Canceled
}