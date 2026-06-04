using System.Net.Http;
using System.Xml.Linq;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.WebBrowser.Abstractions.ChromeWebStore;

namespace ZYC.Framework.Modules.WebBrowser.ChromeWebStore;

[RegisterSingleInstanceAs(typeof(IChromeWebStoreExtensionPackageMetadataProvider))]
internal sealed class ChromeWebStoreExtensionPackageMetadataProvider :
    IChromeWebStoreExtensionPackageMetadataProvider,
    IDisposable
{
    private const string UpdateServiceUri =
        "https://clients2.google.com/service/update2/crx?response=updatecheck&prodversion=120.0.0.0&acceptformat=crx2,crx3&x=id%3D{0}%26uc";

    private HttpClient HttpClient { get; } = new()
    {
        Timeout = TimeSpan.FromMinutes(5)
    };

    public async Task<ChromeWebStoreExtensionPackageMetadata> GetPackageMetadataAsync(
        string extensionId,
        CancellationToken cancellationToken = default)
    {
        var normalizedExtensionId = ChromeWebStoreExtensionId.Normalize(extensionId);
        var requestUri = string.Format(UpdateServiceUri, normalizedExtensionId);
        var responseText = await HttpClient.GetStringAsync(requestUri, cancellationToken);
        return ParseMetadata(normalizedExtensionId, responseText);
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }

    private static ChromeWebStoreExtensionPackageMetadata ParseMetadata(
        string extensionId,
        string responseText)
    {
        var document = XDocument.Parse(responseText);
        var appElement = document.Descendants()
            .FirstOrDefault(t => string.Equals(t.Name.LocalName, "app", StringComparison.OrdinalIgnoreCase));
        var updateCheckElement = appElement?.Elements()
            .FirstOrDefault(t => string.Equals(t.Name.LocalName, "updatecheck", StringComparison.OrdinalIgnoreCase));

        var metadata = new ChromeWebStoreExtensionPackageMetadata
        {
            ExtensionId = extensionId,
            StoreUrl = ChromeWebStoreExtensionId.CreateStoreUrl(extensionId),
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
