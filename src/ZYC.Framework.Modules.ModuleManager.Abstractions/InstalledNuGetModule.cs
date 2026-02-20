namespace ZYC.Framework.Modules.ModuleManager.Abstractions;

/// <summary>
///     Represents a module recorded in the manifest.
/// </summary>
public class InstalledNuGetModule
{
    /// <summary>
    ///     Gets or sets the NuGet package identifier.
    /// </summary>
    public string PackageId { get; set; } = "";

    /// <summary>
    ///     Gets or sets the package version.
    /// </summary>
    public string Version { get; set; } = "";
}