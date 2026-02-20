using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Stores task manager state.
/// </summary>
public class TaskManagerState : IState
{
    /// <summary>
    ///     Gets or sets the active filter type.
    /// </summary>
    public FilterType FilterType { get; set; } = FilterType.All;
}