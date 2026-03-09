using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using ZYC.CoreToolkit.Dotnet;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.NuGet.Abstractions;

namespace ZYC.Framework.Modules.NuGet;

[RegisterSingleInstanceAs(typeof(INuGetManager))]
internal class NuGetManager : INuGetManager, IDisposable
{
    private NuGetSource? _nugetSource;

    public NuGetManager(
        ILogger<NuGetManager> logger,
        IAppContext appContext,
        NuGetConfig config)
    {
        Logger = logger;
        AppContext = appContext;
        NuGetConfig = config;

        SourceCacheContext = new NullSourceCacheContext
        {
            DirectDownload = true
        };

        config.ObserveProperty(nameof(NuGetConfig.Source)).Subscribe(_ =>
        {
            _nugetSource = null;
        }).DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private ILogger<NuGetManager> Logger { get; }
    private IAppContext AppContext { get; }

    private NuGetConfig NuGetConfig { get; }


    public NuGetSource NuGetSource
    {
        get
        {
            if (_nugetSource == null)
            {
                var source = NuGetConfig.Source;
                if (string.IsNullOrWhiteSpace(source))
                {
                    source = "https://api.nuget.org/v3/index.json";
                }

                _nugetSource = new NuGetSource(source);
            }

            return _nugetSource;
        }
    }

    private SourceCacheContext SourceCacheContext { get; }

    public void Dispose()
    {
        CompositeDisposable.Dispose();
        SourceCacheContext.Dispose();
    }

    public async Task ClearNuGetHttpCacheAsync()
    {
        await DotnetNuGetTools.ClearNuGetHttpCacheAsync();
    }

    public async Task DownloadPackageAsync(
        string packageId,
        NuGetVersion nugetVersion,
        CancellationToken token)
    {
        var nugetSource = NuGetSource;

        var downloadResource =
            await nugetSource.SourceRepository.GetResourceAsync<DownloadResource>(token);

        var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
            new PackageIdentity(packageId, nugetVersion),
            new PackageDownloadContext(SourceCacheContext),
            AppContext.GetTempPath(),
            NullLogger.Instance,
            token);

        if (downloadResult.Status != DownloadResourceResultStatus.Available)
        {
            throw new FileNotFoundException($"Download package <{packageId}> failed.");
        }

        downloadResult.PackageStream.CopyToFile(
            GetNuGetPackageCacheFilePath(packageId, nugetVersion.ToString()));
    }

    public string GetNuGetPackageCacheFilePath(string packageId, string nugetVersion)
    {
        return Path.Combine(
            GetCachePath(),
            $"{packageId}.{nugetVersion}.nupkg");
    }

    public async Task<IPackageSearchMetadata> GetPackageMetadataAsync(
        string packageId,
        NuGetVersion nugetVersion,
        CancellationToken token)
    {
        var nugetSource = NuGetSource;

        var packageMetadataResource =
            await nugetSource.SourceRepository.GetResourceAsync<PackageMetadataResource>(token);

        var metadata = await packageMetadataResource.GetMetadataAsync(
            new PackageIdentity(packageId, nugetVersion),
            SourceCacheContext,
            NullLogger.Instance,
            token);

        return metadata;
    }

    public string GetCachePath()
    {
        return Path.Combine(AppContext.GetSettingsDirectory(), NuGetConfig.CacheFolder);
    }

    public async Task DownloadPackageAndDependenciesRecursiveAsync(
        string packageId,
        string version,
        HashSet<string> processedPackages,
        CancellationToken token)
    {
        string ProcessPackage(string id, string v)
        {
            return $"{id}.{v}";
        }

        if (processedPackages.Contains(ProcessPackage(packageId, version)))
        {
            return;
        }

        var nugetVersion = new NuGetVersion(version);

        if (!File.Exists(GetNuGetPackageCacheFilePath(packageId, version)))
        {
            await DownloadPackageAsync(packageId, nugetVersion, token);
        }

        processedPackages.Add(ProcessPackage(packageId, version));

        var nugetSource = NuGetSource;

        var packageMetadataResource =
            await nugetSource.SourceRepository.GetResourceAsync<PackageMetadataResource>(token);

        var metadata = await packageMetadataResource.GetMetadataAsync(
            new PackageIdentity(packageId, nugetVersion),
            SourceCacheContext,
            NullLogger.Instance,
            token);

        foreach (var dependencyGroup in metadata.DependencySets)
        {
            // !WARNING dependencyGroup.TargetFramework
            if (dependencyGroup.TargetFramework.ToString() == ProductInfoExtended.TargetFramework
                || dependencyGroup.TargetFramework.ToString().Contains(".NETStandard"))
            {
                foreach (var dependency in dependencyGroup.Packages)
                {
                    var dependencyVersion = dependency.VersionRange.MinVersion?.ToString();
                    if (dependencyVersion != null)
                    {
                        await DownloadPackageAndDependenciesRecursiveAsync(
                            dependency.Id,
                            dependencyVersion,
                            processedPackages,
                            token);
                    }
                }
            }
        }
    }

    public async Task<NuGetVersion> GetSearchMetadataAsync(
        string packageId,
        bool includePrerelease,
        CancellationToken token)
    {
        var nugetSource = NuGetSource;

        var metadataResource =
            await nugetSource.SourceRepository.GetResourceAsync<MetadataResource>(token);

        var searchMetadata = await metadataResource.GetLatestVersion(
            packageId,
            includePrerelease,
            false,
            SourceCacheContext,
            NullLogger.Instance,
            token);

        if (searchMetadata == null)
        {
            throw new FileNotFoundException("Can't find product from any configured source.");
        }

        return searchMetadata;
    }


    public async Task<string?> FetchReleaseNotesAsync(
        string packageId,
        string version)
    {
        string? patchNote;

        if (UriTools.LooksLikeWindowsPath(NuGetSource.Source))
        {
            patchNote = await TryFetchReleaseNotesFromLocalSourceAsync(
                packageId,
                version);
        }
        else
        {
            patchNote = await TryFetchReleaseNotesAsync(
                packageId,
                version);
        }

        if (!string.IsNullOrWhiteSpace(patchNote))
        {
            return patchNote;
        }

        return null;
    }

    private async Task<string?> TryFetchReleaseNotesAsync(
        string packageId,
        string version)
    {
        try
        {
            using var httpClient = new HttpClient();

            using var indexJson = JsonDocument.Parse(
                await httpClient.GetStringAsync(NuGetSource.Source, CancellationToken.None));
            var baseAddress = indexJson.RootElement
                .GetProperty("resources")
                .EnumerateArray()
                .Select(r => new
                {
                    Type = r.GetProperty("@type").GetString(),
                    Id = r.GetProperty("@id").GetString()
                })
                .First(x => x.Type != null &&
                            x.Type.StartsWith("PackageBaseAddress", StringComparison.OrdinalIgnoreCase))
                .Id!;

            var lowerId = packageId.ToLowerInvariant();
            var nugetVersion = NuGetVersion.Parse(version);
            var lowerVersion = nugetVersion.ToNormalizedString().ToLowerInvariant();

            var nuspecUrl = $"{baseAddress.TrimEnd('/')}/{lowerId}/{lowerVersion}/{lowerId}.nuspec";

            var xml = await httpClient.GetStringAsync(nuspecUrl, CancellationToken.None);
            var xdoc = XDocument.Parse(xml);

            var releaseNotes = xdoc
                .Descendants()
                .FirstOrDefault(e => e.Name.LocalName.Equals("releaseNotes", StringComparison.OrdinalIgnoreCase))
                ?.Value.Trim();

            return string.IsNullOrWhiteSpace(releaseNotes) ? null : releaseNotes;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return e.ToString();
        }
    }

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