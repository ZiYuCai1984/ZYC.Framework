namespace ZYC.Framework.Modules.Update.Abstractions;

/// <summary>
///     Describes the lifecycle/status of the update workflow.
/// </summary>
/// <remarks>
///     Values are typically used to drive UI state (buttons, progress indicators) and to determine
///     which update operation is currently allowed.
/// </remarks>
public enum UpdateStatus
{
    /// <summary>
    ///     The update system is idle and free to start a new operation.
    /// </summary>
    Free,

    /// <summary>
    ///     Checking for a new update.
    /// </summary>
    Checking,

    /// <summary>
    ///     The update check operation failed.
    /// </summary>
    CheckUpdateFaulted,

    /// <summary>
    ///     The update check operation was canceled.
    /// </summary>
    CheckUpdateCanceled,

    /// <summary>
    ///     A newer update is available.
    /// </summary>
    UpdateAvailable,

    /// <summary>
    ///     Downloading the update payload.
    /// </summary>
    Downloading,

    /// <summary>
    ///     The download operation failed.
    /// </summary>
    DownloadFaulted,

    /// <summary>
    ///     Applying the update (install/patch/replace).
    /// </summary>
    Applying,

    /// <summary>
    ///     The update has been staged and is pending application (waiting for user confirmation or a suitable time).
    /// </summary>
    ApplyPending,

    /// <summary>
    ///     The apply operation failed.
    /// </summary>
    ApplyFaulted,

    /// <summary>
    ///     No update is available; the product is up to date.
    /// </summary>
    UpToDate,

    /// <summary>
    ///     A restart is required to complete the update.
    /// </summary>
    RestartPending
}