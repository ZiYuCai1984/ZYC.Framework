using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions;

/// <summary>
///     Tracks installed NuGet modules
/// </summary>
public class NuGetModuleState : IState
{
    /// <summary>
    ///     Gets or sets the installed modules tracked by the manifest.
    /// </summary>
    public InstalledNuGetModule[] InstalledModules { get; set; } = [];
}