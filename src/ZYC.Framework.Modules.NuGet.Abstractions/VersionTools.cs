using NuGet.Versioning;

namespace ZYC.Framework.Modules.NuGet.Abstractions;

/// <summary>
///     Provides helper methods for comparing NuGet versions.
/// </summary>
public static class VersionTools
{
    /// <summary>
    ///     Determines whether <paramref name="version1" /> is newer than <paramref name="version2" />.
    /// </summary>
    /// <param name="version1">The version to compare.</param>
    /// <param name="version2">The version to compare against.</param>
    /// <returns><c>true</c> if <paramref name="version1" /> is newer; otherwise, <c>false</c>.</returns>
    public static bool IsNew(string version1, string version2)
    {
        var v1 = new NuGetVersion(version1);
        var v2 = new NuGetVersion(version2);

        return v1 > v2;
    }

    /// <summary>
    ///     Determines whether <paramref name="version1" /> is newer than <paramref name="version2" />.
    /// </summary>
    /// <param name="version1">The version to compare.</param>
    /// <param name="version2">The version to compare against.</param>
    /// <returns><c>true</c> if <paramref name="version1" /> is newer; otherwise, <c>false</c>.</returns>
    public static bool IsNew(NuGetVersion version1, string version2)
    {
        var v2 = new NuGetVersion(version2);
        return version1 > v2;
    }

    // ReSharper disable once UnusedMember.Local
    /// <summary>
    ///     Determines whether <paramref name="version1" /> is newer than <paramref name="version2" />.
    /// </summary>
    /// <param name="version1">The version to compare.</param>
    /// <param name="version2">The version to compare against.</param>
    /// <returns><c>true</c> if <paramref name="version1" /> is newer; otherwise, <c>false</c>.</returns>
    public static bool IsNew(string version1, NuGetVersion version2)
    {
        var v1 = new NuGetVersion(version1);
        return v1 > version2;
    }
}