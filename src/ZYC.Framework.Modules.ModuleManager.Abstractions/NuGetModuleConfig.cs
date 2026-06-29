using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions;

/// <summary>
///     Configuration for NuGet-based module discovery and loading.
/// </summary>
public class NuGetModuleConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the search term used when querying NuGet packages
    ///     for available modules.
    /// </summary>
    public string SearchTerm { get; set; } = "ZYC.Framework.Modules.";

    /// <summary>
    ///     Gets or sets the requested maximum number of NuGet search results.
    /// </summary>
    /// <remarks>
    ///     NuGet.org limits a single search request to 1000 results; larger
    ///     configured values are clamped before querying.
    /// </remarks>
    public int SearchTake { get; set; } = 1000;

    /// <summary>
    ///     Gets or sets the number of search results to skip when performing
    ///     the NuGet query. This can be used for pagination.
    /// </summary>
    public int SearchSkip { get; set; } = 0;

    /// <summary>
    ///     Gets or sets the target framework used to filter compatible modules.
    /// </summary>
    public string TargetFramework { get; set; } = "net10.0-windows";

    /// <summary>
    ///     Gets or sets the regular expression used to filter module package names.
    /// </summary>
    public string IncludeRegex { get; set; } = "^(?!.*Abstractions$).+";
}