using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Update.Abstractions;

/// <summary>
///     Configuration options for the application update system.
/// </summary>
public class UpdateConfig : IConfig
{
    /// <summary>
    ///     Whether to check for updates when the application starts.
    ///     In DEBUG builds this defaults to <c>false</c> to avoid slowing down inner-loop development.
    /// </summary>
    public bool CheckAtStartup { get; set; }

#if DEBUG
        = false;
#else
        = true;
#endif

    /// <summary>
    ///     Whether prerelease versions (e.g., alpha/beta/rc) should be included when checking for updates.
    /// </summary>
    public bool IncludePrerelease { get; set; } = true;
}