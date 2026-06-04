namespace ZYC.Framework.Modules.WebBrowser.ChromeWebStore;

internal static class ChromeWebStoreExtensionId
{
    private const int ExtensionIdLength = 32;
    private const string ChromeWebStoreHost = "chromewebstore.google.com";

    public static string Normalize(string value)
    {
        if (TryParseFromStoreUri(value, out var extensionId))
        {
            return extensionId;
        }

        extensionId = value.Trim().ToLowerInvariant();
        if (!IsValid(extensionId))
        {
            throw new ArgumentException($"Invalid Chrome Web Store extension id <{value}>.", nameof(value));
        }

        return extensionId;
    }

    public static bool TryParseFromStoreUri(string value, out string extensionId)
    {
        if (Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri))
        {
            return TryParseFromStoreUri(uri, out extensionId);
        }

        extensionId = "";
        return false;
    }

    public static bool TryParseFromStoreUri(Uri uri, out string extensionId)
    {
        extensionId = "";
        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(uri.Host, ChromeWebStoreHost, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var segments = uri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var detailIndex = Array.FindIndex(segments, t => string.Equals(t, "detail", StringComparison.OrdinalIgnoreCase));
        if (detailIndex < 0)
        {
            return false;
        }

        foreach (var segment in segments.Skip(detailIndex + 1).Reverse())
        {
            var candidate = segment.Trim().ToLowerInvariant();
            if (IsValid(candidate))
            {
                extensionId = candidate;
                return true;
            }
        }

        return false;
    }

    public static bool IsValid(string value)
    {
        if (value.Length != ExtensionIdLength)
        {
            return false;
        }

        return value.All(c => c is >= 'a' and <= 'p');
    }

    public static string CreateStoreUrl(string extensionId)
    {
        return $"https://{ChromeWebStoreHost}/detail/{Normalize(extensionId)}";
    }
}
