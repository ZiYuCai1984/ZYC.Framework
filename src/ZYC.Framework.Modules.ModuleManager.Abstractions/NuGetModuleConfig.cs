using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions;

/// <summary>
///     Configuration for the NuGet modules.
/// </summary>
public class NuGetModuleConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the search term used for module discovery.
    /// </summary>
    public string SearchTerm { get; set; } = "ZYC.Framework.Modules.";

    public int SearchTake { get; set; } = 1024;

    public int SearchSkip { get; set; } = 0;

    public string TargetFramework { get; set; } = "net10.0-windows";

    public string IncludeRegex { get; set; } = "^(?!.*Abstractions$).+";
}