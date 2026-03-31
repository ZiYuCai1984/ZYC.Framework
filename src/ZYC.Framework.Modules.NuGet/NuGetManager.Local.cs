using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using NuGet.Versioning;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.NuGet;

internal partial class NuGetManager
{
    /// <summary>
    ///     !WARNING Powered by chatgpt 5.4
    /// </summary>
    private async Task<string?> TryFetchReleaseNotesFromLocalSourceAsync(
        string packageId,
        string version)
    {
        try
        {
            var localSourcePath = NuGetSource.Source;


            if (string.IsNullOrWhiteSpace(localSourcePath))
            {
                return null;
            }

            if (!Directory.Exists(localSourcePath))
            {
                return null;
            }

            var nugetVersion = NuGetVersion.Parse(version);
            var normalizedVersion = nugetVersion.ToNormalizedString();

            var packageFile = FindLocalNupkg(localSourcePath, packageId, normalizedVersion);
            if (packageFile == null)
            {
                return null;
            }

            await using var stream = File.OpenRead(packageFile);
            await using var archive = new ZipArchive(stream, ZipArchiveMode.Read, false);

            var nuspecEntry = archive.Entries.FirstOrDefault(x =>
                x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));

            if (nuspecEntry == null)
            {
                return null;
            }

            await using var nuspecStream = await nuspecEntry.OpenAsync();
            var xdoc = await XDocument.LoadAsync(nuspecStream, LoadOptions.None, CancellationToken.None);

            var releaseNotes = xdoc
                .Descendants()
                .FirstOrDefault(e => e.Name.LocalName.Equals("releaseNotes", StringComparison.OrdinalIgnoreCase))
                ?.Value
                .Trim();

            return string.IsNullOrWhiteSpace(releaseNotes) ? null : releaseNotes;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return e.ToString();
        }
    }

    private static string? FindLocalNupkg(
        string localSourcePath,
        string packageId,
        string normalizedVersion)
    {
        var lowerId = packageId.ToLowerInvariant();
        var lowerVersion = normalizedVersion.ToLowerInvariant();

        var expectedFileName = $"{packageId}.{normalizedVersion}.nupkg";

        var exact = Directory.EnumerateFiles(localSourcePath, "*.nupkg", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(x => string.Equals(
                Path.GetFileName(x),
                expectedFileName,
                StringComparison.OrdinalIgnoreCase));

        if (exact != null)
        {
            return exact;
        }

        var hierarchicalDir = Path.Combine(localSourcePath, packageId, normalizedVersion);
        if (Directory.Exists(hierarchicalDir))
        {
            var hierarchicalFile = Directory.EnumerateFiles(hierarchicalDir, "*.nupkg", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
            if (hierarchicalFile != null)
            {
                return hierarchicalFile;
            }
        }

        var idDir = Directory.EnumerateDirectories(localSourcePath, "*", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(x => string.Equals(
                Path.GetFileName(x),
                packageId,
                StringComparison.OrdinalIgnoreCase));

        if (idDir != null)
        {
            var versionDir = Directory.EnumerateDirectories(idDir, "*", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(x => string.Equals(
                    Path.GetFileName(x),
                    normalizedVersion,
                    StringComparison.OrdinalIgnoreCase));

            if (versionDir != null)
            {
                var versionFile = Directory.EnumerateFiles(versionDir, "*.nupkg", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();
                if (versionFile != null)
                {
                    return versionFile;
                }
            }
        }

        var fallback = Directory.EnumerateFiles(localSourcePath, "*.nupkg", SearchOption.AllDirectories)
            .FirstOrDefault(x =>
            {
                var fileName = Path.GetFileNameWithoutExtension(x);
                return fileName.Equals($"{packageId}.{normalizedVersion}", StringComparison.OrdinalIgnoreCase);
            });

        if (fallback != null)
        {
            return fallback;
        }

        return Directory.EnumerateFiles(localSourcePath, "*.nupkg", SearchOption.AllDirectories)
            .FirstOrDefault(x =>
            {
                var fileName = Path.GetFileNameWithoutExtension(x).ToLowerInvariant();
                return fileName.StartsWith(lowerId + ".", StringComparison.OrdinalIgnoreCase)
                       && fileName.Contains(lowerVersion, StringComparison.OrdinalIgnoreCase);
            });
    }
}