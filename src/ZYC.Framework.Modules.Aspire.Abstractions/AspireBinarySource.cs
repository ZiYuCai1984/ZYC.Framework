namespace ZYC.Framework.Modules.Aspire.Abstractions;

/// <summary>
///     Defines where Aspire binaries are resolved from.
/// </summary>
public enum AspireBinarySource
{
    /// <summary>
    ///     Resolve binaries from the NuGet cache.
    /// </summary>
    NuGetCache,

    /// <summary>
    ///     Resolve binaries from the application install folder.
    /// </summary>
    ApplicationFolder,

    /// <summary>
    ///     Resolve binaries from custom paths provided by the user.
    /// </summary>
    Custom
}