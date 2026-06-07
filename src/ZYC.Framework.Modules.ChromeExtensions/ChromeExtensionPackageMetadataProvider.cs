using System.Net.Http;
using System.Xml.Linq;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;

namespace ZYC.Framework.Modules.ChromeExtensions;

[RegisterSingleInstanceAs(typeof(IChromeExtensionPackageMetadataProvider))]
internal class ChromeExtensionPackageMetadataProvider :
    IChromeExtensionPackageMetadataProvider,
    IDisposable
{
    public ChromeExtensionPackageMetadataProvider(ChromeExtensionManagerConfig chromeExtensionManagerConfig)
    {
        ChromeExtensionManagerConfig = chromeExtensionManagerConfig;
    }

    private ChromeExtensionManagerConfig ChromeExtensionManagerConfig { get; }

    private HttpClient HttpClient { get; } = new()
    {
        Timeout = TimeSpan.FromMinutes(5)
    };

    public async Task<ChromeExtensionPackageMetadata> GetPackageMetadataAsync(
        string extensionId,
        CancellationToken cancellationToken = default)
    {
        var normalizedExtensionId = ChromeExtensionId.Normalize(extensionId);
        var requestUri = string.Format(ChromeExtensionManagerConfig.UpdateServiceUri, normalizedExtensionId);
        var responseText = await HttpClient.GetStringAsync(requestUri, cancellationToken);
        return ParseMetadata(normalizedExtensionId, responseText);
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }

    private static ChromeExtensionPackageMetadata ParseMetadata(
        string extensionId,
        string responseText)
    {
        var document = XDocument.Parse(responseText);
        var appElement = document.Descendants()
            .FirstOrDefault(t => string.Equals(t.Name.LocalName, "app", StringComparison.OrdinalIgnoreCase));
        var updateCheckElement = appElement?.Elements()
            .FirstOrDefault(t => string.Equals(t.Name.LocalName, "updatecheck", StringComparison.OrdinalIgnoreCase));

        var metadata = new ChromeExtensionPackageMetadata
        {
            ExtensionId = extensionId,
            StoreUrl = ChromeExtensionId.CreateStoreUrl(extensionId),
            AppStatus = ReadAttribute(appElement, "status"),
            UpdateCheckStatus = ReadAttribute(updateCheckElement, "status"),
            Version = ReadAttribute(updateCheckElement, "version"),
            DownloadUrl = ReadAttribute(updateCheckElement, "codebase"),
            HashSha256 = ReadAttribute(updateCheckElement, "hash_sha256"),
            Fingerprint = ReadAttribute(updateCheckElement, "fp")
        };

        if (long.TryParse(ReadAttribute(updateCheckElement, "size"), out var sizeBytes))
        {
            metadata.SizeBytes = sizeBytes;
        }

        return metadata;
    }

    private static string? ReadAttribute(XElement? element, string name)
    {
        return element?.Attribute(name)?.Value;
    }
}