using System.Security.Cryptography;

namespace ZYC.Framework.Modules.ChromeExtensions;

internal static class ChromeExtensionId
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

    public static string FromPublicKey(ReadOnlySpan<byte> publicKey)
    {
        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(publicKey, hash);
        return FromCrxIdBytes(hash[..16]);
    }

    public static string FromCrxIdBytes(ReadOnlySpan<byte> crxId)
    {
        if (crxId.Length != ExtensionIdLength / 2)
        {
            throw new ArgumentException(
                $"Invalid CRX id byte length <{crxId.Length}>.",
                nameof(crxId));
        }

        Span<char> chars = stackalloc char[ExtensionIdLength];
        for (var i = 0; i < crxId.Length; i++)
        {
            chars[i * 2] = (char)('a' + ((crxId[i] >> 4) & 0x0F));
            chars[i * 2 + 1] = (char)('a' + (crxId[i] & 0x0F));
        }

        return new string(chars);
    }

    public static string CreateStoreUrl(string extensionId)
    {
        return $"https://{ChromeWebStoreHost}/detail/{Normalize(extensionId)}";
    }
}
