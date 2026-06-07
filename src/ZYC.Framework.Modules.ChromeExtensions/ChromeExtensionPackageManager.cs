using System.Buffers.Binary;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;

namespace ZYC.Framework.Modules.ChromeExtensions;

[RegisterSingleInstanceAs(typeof(IChromeExtensionPackageManager))]
internal class ChromeExtensionPackageManager : IChromeExtensionPackageManager, IDisposable
{
    private readonly SemaphoreSlim _operationLock = new(1, 1);

    public ChromeExtensionPackageManager(
        IAppContext appContext,
        ChromeExtensionManagerConfig managerConfig,
        IChromeExtensionPackageMetadataProvider packageMetadataProvider)
    {
        AppContext = appContext;
        ManagerConfig = managerConfig;
        PackageMetadataProvider = packageMetadataProvider;
    }

    private IAppContext AppContext { get; }

    private ChromeExtensionManagerConfig ManagerConfig { get; }

    private IChromeExtensionPackageMetadataProvider PackageMetadataProvider { get; }

    private HttpClient HttpClient { get; } = new()
    {
        Timeout = TimeSpan.FromMinutes(5)
    };

    private string PackagesRoot => Path.Combine(
        AppContext.GetSettingsDirectory(),
        "ChromeExtensions");

    public IReadOnlyList<ChromeInstalledExtension> GetInstalledExtensions()
    {
        RefreshManifestInfo();
        return ManagerConfig.InstalledExtensions
            .OrderBy(t => t.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(t => t.ExtensionId, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public ChromeInstalledExtension? GetInstalledExtension(string extensionId)
    {
        var normalizedExtensionId = ChromeExtensionId.Normalize(extensionId);
        return GetInstalledExtensions().FirstOrDefault(t =>
            string.Equals(t.ExtensionId, normalizedExtensionId, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsInstalled(string extensionId)
    {
        var normalizedExtensionId = ChromeExtensionId.Normalize(extensionId);
        return ManagerConfig.InstalledExtensions.Any(t =>
            string.Equals(t.ExtensionId, normalizedExtensionId, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<ChromeInstalledExtension> InstallAsync(
        string extensionId,
        CancellationToken cancellationToken = default)
    {
        var normalizedExtensionId = ChromeExtensionId.Normalize(extensionId);

        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            var metadata = await PackageMetadataProvider.GetPackageMetadataAsync(normalizedExtensionId, cancellationToken);
            if (!metadata.HasPackage)
            {
                throw new InvalidOperationException(
                    $"Chrome Web Store update service returned no package for <{normalizedExtensionId}>. AppStatus=<{metadata.AppStatus}>, UpdateCheckStatus=<{metadata.UpdateCheckStatus}>.");
            }

            var packagePath = GetPackagePath(metadata);
            await DownloadPackageAsync(
                metadata.DownloadUrl!,
                packagePath,
                metadata.HashSha256,
                cancellationToken);

            var unpackedPath = GetUnpackedPath(metadata);
            await ExtractPackageAsync(packagePath, unpackedPath, cancellationToken);
            var manifestInfo = ReadExtensionManifestInfo(unpackedPath, metadata.ExtensionId);

            var installed = new ChromeInstalledExtension
            {
                Name = manifestInfo.Name,
                ExtensionId = metadata.ExtensionId,
                StoreUrl = metadata.StoreUrl,
                Version = metadata.Version ?? "",
                DownloadUrl = metadata.DownloadUrl ?? "",
                PackagePath = packagePath,
                UnpackedPath = unpackedPath,
                PopupPagePath = manifestInfo.PopupPagePath,
                PopupPageUrl = manifestInfo.PopupPageUrl,
                OptionsPagePath = manifestInfo.OptionsPagePath,
                OptionsPageUrl = manifestInfo.OptionsPageUrl,
                SizeBytes = metadata.SizeBytes,
                HashSha256 = metadata.HashSha256,
                Fingerprint = metadata.Fingerprint,
                InstalledAt = DateTimeOffset.UtcNow
            };

            ManagerConfig.InstalledExtensions = ManagerConfig.InstalledExtensions
                .Where(t => !string.Equals(t.ExtensionId, installed.ExtensionId, StringComparison.OrdinalIgnoreCase))
                .Append(installed)
                .OrderBy(t => t.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(t => t.ExtensionId, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            DeleteOldVersionFolders(installed.ExtensionId, Path.GetDirectoryName(installed.PackagePath));
            AppContext.SaveAllConfig();

            return installed;
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task<bool> UninstallAsync(string extensionId)
    {
        var normalizedExtensionId = ChromeExtensionId.Normalize(extensionId);

        await _operationLock.WaitAsync();
        try
        {
            if (!ManagerConfig.InstalledExtensions.Any(t =>
                    string.Equals(t.ExtensionId, normalizedExtensionId, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            ManagerConfig.InstalledExtensions = ManagerConfig.InstalledExtensions
                .Where(t => !string.Equals(t.ExtensionId, normalizedExtensionId, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            DeleteExtensionPackageFolder(normalizedExtensionId);
            AppContext.SaveAllConfig();

            return true;
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        _operationLock.Dispose();
    }

    private void RefreshManifestInfo()
    {
        var changed = false;
        foreach (var installed in ManagerConfig.InstalledExtensions)
        {
            changed |= TryRefreshManifestInfo(installed);
        }

        if (changed)
        {
            AppContext.SaveAllConfig();
        }
    }

    private string GetPackagePath(ChromeExtensionPackageMetadata metadata)
    {
        var version = SanitizePathSegment(metadata.Version ?? "unknown");
        var packageFolder = Path.Combine(PackagesRoot, metadata.ExtensionId, version);
        Directory.CreateDirectory(packageFolder);

        return Path.Combine(packageFolder, $"{metadata.ExtensionId}_{version}.crx");
    }

    private string GetUnpackedPath(ChromeExtensionPackageMetadata metadata)
    {
        var version = SanitizePathSegment(metadata.Version ?? "unknown");
        return Path.Combine(PackagesRoot, metadata.ExtensionId, version, "unpacked");
    }

    private async Task DownloadPackageAsync(
        string downloadUrl,
        string packagePath,
        string? expectedHashSha256,
        CancellationToken cancellationToken)
    {
        var tempPath = $"{packagePath}.tmp";
        try
        {
            using var response = await HttpClient.GetAsync(
                downloadUrl,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            response.EnsureSuccessStatusCode();

            await using (var input = await response.Content.ReadAsStreamAsync(cancellationToken))
            await using (var output = File.Create(tempPath))
            {
                await input.CopyToAsync(output, cancellationToken);
            }

            await VerifyPackageHashAsync(tempPath, expectedHashSha256, cancellationToken);

            File.Move(tempPath, packagePath, true);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    private void DeleteExtensionPackageFolder(string extensionId)
    {
        var root = Path.GetFullPath(PackagesRoot);
        var extensionRoot = Path.GetFullPath(Path.Combine(root, extensionId));
        if (!extensionRoot.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Refusing to delete package folder outside <{root}>.");
        }

        if (Directory.Exists(extensionRoot))
        {
            Directory.Delete(extensionRoot, true);
        }
    }

    private void DeleteOldVersionFolders(string extensionId, string? currentVersionFolder)
    {
        if (string.IsNullOrWhiteSpace(currentVersionFolder))
        {
            return;
        }

        var root = Path.GetFullPath(PackagesRoot);
        var extensionRoot = Path.GetFullPath(Path.Combine(root, extensionId));
        var currentFolder = Path.GetFullPath(currentVersionFolder);
        if (!extensionRoot.StartsWith(root, StringComparison.OrdinalIgnoreCase)
            || !currentFolder.StartsWith(extensionRoot, StringComparison.OrdinalIgnoreCase)
            || !Directory.Exists(extensionRoot))
        {
            return;
        }

        foreach (var versionFolder in Directory.EnumerateDirectories(extensionRoot))
        {
            var fullVersionFolder = Path.GetFullPath(versionFolder);
            if (!string.Equals(fullVersionFolder, currentFolder, StringComparison.OrdinalIgnoreCase))
            {
                Directory.Delete(fullVersionFolder, true);
            }
        }
    }

    private static bool TryRefreshManifestInfo(ChromeInstalledExtension installed)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(installed.UnpackedPath)
                || !Directory.Exists(installed.UnpackedPath)
                || !File.Exists(Path.Combine(installed.UnpackedPath, "manifest.json")))
            {
                return false;
            }

            var manifestInfo = ReadExtensionManifestInfo(installed.UnpackedPath, installed.ExtensionId);
            var changed = false;
            changed |= UpdateValueIfChanged(value => installed.Name = value, installed.Name, manifestInfo.Name);
            changed |= UpdateValueIfChanged(
                value => installed.PopupPagePath = value,
                installed.PopupPagePath,
                manifestInfo.PopupPagePath);
            changed |= UpdateValueIfChanged(
                value => installed.PopupPageUrl = value,
                installed.PopupPageUrl,
                manifestInfo.PopupPageUrl);
            changed |= UpdateValueIfChanged(
                value => installed.OptionsPagePath = value,
                installed.OptionsPagePath,
                manifestInfo.OptionsPagePath);
            changed |= UpdateValueIfChanged(
                value => installed.OptionsPageUrl = value,
                installed.OptionsPageUrl,
                manifestInfo.OptionsPageUrl);

            return changed;
        }
        catch
        {
            return false;
        }
    }

    private static bool UpdateValueIfChanged(
        Action<string> update,
        string? currentValue,
        string? newValue)
    {
        newValue ??= "";
        if (string.Equals(currentValue ?? "", newValue, StringComparison.Ordinal))
        {
            return false;
        }

        update(newValue);
        return true;
    }

    private static string SanitizePathSegment(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var chars = value.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray();
        return new string(chars);
    }

    private static async Task VerifyPackageHashAsync(
        string packagePath,
        string? expectedHashSha256,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(expectedHashSha256))
        {
            return;
        }

        string actualHashSha256;
        await using (var stream = File.OpenRead(packagePath))
        {
            var hash = await SHA256.HashDataAsync(stream, cancellationToken);
            actualHashSha256 = Convert.ToHexString(hash).ToLowerInvariant();
        }

        if (!string.Equals(actualHashSha256, expectedHashSha256, StringComparison.OrdinalIgnoreCase))
        {
            File.Delete(packagePath);
            throw new InvalidOperationException(
                $"Downloaded package hash mismatch. Expected <{expectedHashSha256}>, actual <{actualHashSha256}>.");
        }
    }

    private static async Task ExtractPackageAsync(
        string packagePath,
        string unpackedPath,
        CancellationToken cancellationToken)
    {
        var tempZipPath = $"{packagePath}.zip.tmp";
        try
        {
            await using (var packageStream = File.OpenRead(packagePath))
            {
                var payloadOffset = ReadCrxZipPayloadOffset(packageStream);
                packageStream.Position = payloadOffset;

                await using var zipStream = File.Create(tempZipPath);
                await packageStream.CopyToAsync(zipStream, cancellationToken);
            }

            ExtractZipArchive(tempZipPath, unpackedPath);
            DeleteWebView2ReservedMetadataFolder(unpackedPath);
        }
        finally
        {
            if (File.Exists(tempZipPath))
            {
                File.Delete(tempZipPath);
            }
        }
    }

    private static long ReadCrxZipPayloadOffset(Stream stream)
    {
        Span<byte> header = stackalloc byte[12];
        stream.ReadExactly(header);

        if (header[0] != (byte)'C'
            || header[1] != (byte)'r'
            || header[2] != (byte)'2'
            || header[3] != (byte)'4')
        {
            throw new InvalidOperationException("The downloaded package is not a CRX file.");
        }

        var version = BinaryPrimitives.ReadUInt32LittleEndian(header[4..8]);
        var offset = version switch
        {
            2 => ReadCrx2ZipPayloadOffset(stream, header),
            3 => 12L + BinaryPrimitives.ReadUInt32LittleEndian(header[8..12]),
            _ => throw new InvalidOperationException($"Unsupported CRX version <{version}>.")
        };

        if (offset <= 0 || offset >= stream.Length)
        {
            throw new InvalidOperationException($"Invalid CRX payload offset <{offset}>.");
        }

        return offset;
    }

    private static long ReadCrx2ZipPayloadOffset(Stream stream, ReadOnlySpan<byte> header)
    {
        Span<byte> signatureLengthBytes = stackalloc byte[4];
        stream.ReadExactly(signatureLengthBytes);

        var publicKeyLength = BinaryPrimitives.ReadUInt32LittleEndian(header[8..12]);
        var signatureLength = BinaryPrimitives.ReadUInt32LittleEndian(signatureLengthBytes);
        return 16L + publicKeyLength + signatureLength;
    }

    private static void ExtractZipArchive(string zipPath, string destinationPath)
    {
        if (Directory.Exists(destinationPath))
        {
            Directory.Delete(destinationPath, true);
        }

        Directory.CreateDirectory(destinationPath);

        var destinationRoot = Path.GetFullPath(destinationPath);
        if (!destinationRoot.EndsWith(Path.DirectorySeparatorChar))
        {
            destinationRoot += Path.DirectorySeparatorChar;
        }

        try
        {
            using var archive = ZipFile.OpenRead(zipPath);
            foreach (var entry in archive.Entries)
            {
                var entryPath = Path.GetFullPath(Path.Combine(destinationPath, entry.FullName));
                if (!entryPath.StartsWith(destinationRoot, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Package entry <{entry.FullName}> points outside the unpacked extension folder.");
                }

                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(entryPath);
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(entryPath)!);
                entry.ExtractToFile(entryPath, true);
            }
        }
        catch
        {
            if (Directory.Exists(destinationPath))
            {
                Directory.Delete(destinationPath, true);
            }

            throw;
        }
    }

    private static void DeleteWebView2ReservedMetadataFolder(string unpackedPath)
    {
        var metadataPath = Path.Combine(unpackedPath, "_metadata");
        if (Directory.Exists(metadataPath))
        {
            Directory.Delete(metadataPath, true);
        }
    }

    private static ChromeWebStoreExtensionManifestInfo ReadExtensionManifestInfo(
        string unpackedPath,
        string extensionId)
    {
        var manifestPath = Path.Combine(unpackedPath, "manifest.json");
        if (!File.Exists(manifestPath))
        {
            throw new InvalidOperationException($"Unpacked extension manifest not found at <{manifestPath}>.");
        }

        using var manifestStream = File.OpenRead(manifestPath);
        using var manifest = JsonDocument.Parse(manifestStream);

        var name = ReadJsonString(manifest.RootElement, "name");
        if (!string.IsNullOrWhiteSpace(name)
            && TryGetLocalizedMessageName(name, out var localizedMessageName))
        {
            name = ResolveLocalizedMessage(unpackedPath, manifest.RootElement, localizedMessageName) ?? name;
        }

        var popupPagePath = ReadPopupPagePath(manifest.RootElement) ?? "";
        var optionsPagePath = ReadOptionsPagePath(manifest.RootElement) ?? "";

        return new ChromeWebStoreExtensionManifestInfo(
            string.IsNullOrWhiteSpace(name) ? extensionId : name,
            popupPagePath,
            CreateExtensionPageUrl(extensionId, popupPagePath),
            optionsPagePath,
            CreateExtensionPageUrl(extensionId, optionsPagePath));
    }

    private static string? ReadPopupPagePath(JsonElement manifest)
    {
        return ReadNestedJsonString(manifest, "action", "default_popup")
               ?? ReadNestedJsonString(manifest, "browser_action", "default_popup")
               ?? ReadNestedJsonString(manifest, "page_action", "default_popup");
    }

    private static string? ReadOptionsPagePath(JsonElement manifest)
    {
        return ReadNestedJsonString(manifest, "options_ui", "page")
               ?? ReadJsonString(manifest, "options_page");
    }

    private static string CreateExtensionPageUrl(string extensionId, string pagePath)
    {
        if (string.IsNullOrWhiteSpace(pagePath))
        {
            return "";
        }

        if (Uri.TryCreate(pagePath, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.Scheme.Equals("chrome-extension", StringComparison.OrdinalIgnoreCase)
                ? absoluteUri.ToString()
                : "";
        }

        var normalizedPagePath = pagePath.Trim().Replace('\\', '/').TrimStart('/');
        return string.IsNullOrWhiteSpace(normalizedPagePath)
            ? ""
            : $"chrome-extension://{extensionId}/{normalizedPagePath}";
    }

    private static string? ResolveLocalizedMessage(
        string unpackedPath,
        JsonElement manifest,
        string messageName)
    {
        foreach (var locale in GetLocaleCandidates(unpackedPath, manifest))
        {
            var messagesPath = Path.Combine(unpackedPath, "_locales", locale, "messages.json");
            if (!File.Exists(messagesPath))
            {
                continue;
            }

            try
            {
                using var messagesStream = File.OpenRead(messagesPath);
                using var messages = JsonDocument.Parse(messagesStream);
                if (!messages.RootElement.TryGetProperty(messageName, out var message)
                    || !message.TryGetProperty("message", out var value)
                    || value.ValueKind != JsonValueKind.String)
                {
                    continue;
                }

                var localizedValue = value.GetString();
                if (!string.IsNullOrWhiteSpace(localizedValue))
                {
                    return localizedValue;
                }
            }
            catch (JsonException)
            {
                continue;
            }
        }

        return null;
    }

    private static IReadOnlyList<string> GetLocaleCandidates(string unpackedPath, JsonElement manifest)
    {
        var candidates = new List<string>();
        AddLocaleCandidate(candidates, ReadJsonString(manifest, "default_locale"));

        var currentCultureName = CultureInfo.CurrentUICulture.Name;
        AddLocaleCandidate(candidates, currentCultureName.Replace('-', '_'));
        AddLocaleCandidate(candidates, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        AddLocaleCandidate(candidates, "en");
        AddLocaleCandidate(candidates, "en_US");

        var localesRoot = Path.Combine(unpackedPath, "_locales");
        if (Directory.Exists(localesRoot))
        {
            foreach (var localeFolder in Directory.EnumerateDirectories(localesRoot))
            {
                AddLocaleCandidate(candidates, Path.GetFileName(localeFolder));
            }
        }

        return candidates;
    }

    private static void AddLocaleCandidate(ICollection<string> candidates, string? locale)
    {
        if (string.IsNullOrWhiteSpace(locale)
            || candidates.Any(t => string.Equals(t, locale, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        candidates.Add(locale);
    }

    private static string? ReadJsonString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value)
            || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return value.GetString();
    }

    private static string? ReadNestedJsonString(
        JsonElement element,
        string parentPropertyName,
        string propertyName)
    {
        if (!element.TryGetProperty(parentPropertyName, out var parent)
            || parent.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return ReadJsonString(parent, propertyName);
    }

    private static bool TryGetLocalizedMessageName(string value, out string messageName)
    {
        const string prefix = "__MSG_";
        const string suffix = "__";

        if (value.StartsWith(prefix, StringComparison.Ordinal)
            && value.EndsWith(suffix, StringComparison.Ordinal)
            && value.Length > prefix.Length + suffix.Length)
        {
            messageName = value[prefix.Length..^suffix.Length];
            return true;
        }

        messageName = "";
        return false;
    }

    private sealed record ChromeWebStoreExtensionManifestInfo(
        string Name,
        string PopupPagePath,
        string PopupPageUrl,
        string OptionsPagePath,
        string OptionsPageUrl);
}
