using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Update.Abstractions;

/// <summary>
///     Runtime state for the update system.
///     Typically used to expose current update progress/status to the UI.
/// </summary>
public class UpdateState : IState
{
    /// <summary>
    ///     Current update status of the application (e.g., Idle, Checking, Downloading, ReadyToInstall).
    /// </summary>
    public UpdateStatus UpdateStatus { get; set; }
}