namespace ZYC.Framework.Modules.Update.Abstractions;

/// <summary>
///     Extra metadata associated with a NuGet package/release.
/// </summary>
public class NuGetAdditionalData
{
    /// <summary>
    ///     Human-readable description of the release/package.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Release notes / patch notes describing what changed in this version.
    /// </summary>
    public string PatchNote { get; set; } = string.Empty;

    /// <summary>
    ///     Direct download URI for the installer/setup file for this release.
    /// </summary>
    public string SetupFileDownloadUri { get; set; } = string.Empty;
}