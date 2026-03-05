using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using ZYC.Framework.Abstractions.MCP;

namespace ZYC.Framework.Modules.NuGet.Abstractions;

/// <summary>
///     Provides NuGet package operations for modules.
/// </summary>
[ExposeToMCP]
public interface INuGetManager
{
    /// <summary>
    ///     Clears the NuGet HTTP cache.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ClearNuGetHttpCacheAsync();

    /// <summary>
    ///     Downloads a NuGet package by ID and version.
    /// </summary>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="nugetVersion">The package version.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DownloadPackageAsync(
        string packageId,
        NuGetVersion nugetVersion,
        CancellationToken token);

    /// <summary>
    ///     Gets the cache file path for a NuGet package.
    /// </summary>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="nugetVersion">The package version string.</param>
    /// <returns>The cache file path.</returns>
    string GetNuGetPackageCacheFilePath(
        string packageId,
        string nugetVersion);


    /// <summary>
    ///     Downloads a package and its dependencies recursively.
    /// </summary>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version string.</param>
    /// <param name="processedPackages">The set of already-processed packages.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DownloadPackageAndDependenciesRecursiveAsync(
        string packageId,
        string version,
        HashSet<string> processedPackages,
        CancellationToken token);


    /// <summary>
    ///     Gets search metadata for a package.
    /// </summary>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="includePrerelease">Whether to include prerelease versions.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The latest package version.</returns>
    Task<NuGetVersion> GetSearchMetadataAsync(
        string packageId,
        bool includePrerelease,
        CancellationToken token);


    /// <summary>
    ///     Gets metadata for a specific package version.
    /// </summary>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The package metadata.</returns>
    Task<IPackageSearchMetadata> GetPackageMetadataAsync(
        string packageId,
        NuGetVersion version,
        CancellationToken token);

    /// <summary>
    ///     Gets the base cache path used by NuGet operations.
    /// </summary>
    /// <returns>The cache path.</returns>
    string GetCachePath();

    /// <summary>
    ///     Fetches release notes for the specified package version.
    /// </summary>
    /// <param name="packageId">The package identifier.</param>
    /// <param name="version">The package version.</param>
    /// <returns>The release notes, if available.</returns>
    Task<string?> FetchReleaseNotesAsync(
        string packageId,
        string version);
}