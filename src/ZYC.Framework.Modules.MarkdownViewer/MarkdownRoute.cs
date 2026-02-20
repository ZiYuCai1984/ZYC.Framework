using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.MarkdownViewer.Abstractions;

namespace ZYC.Framework.Modules.MarkdownViewer;

//TODO-zyc Needs to be refactored!!
public static class MarkdownRoute
{
    public static string Scheme => ProductInfo.Scheme;

    public static string Host => MarkdownViewerModuleConstants.Host;

    public const string UriParam = "src";


    public static Uri BuildEmpty()
    {
        return new Uri($"{Scheme}://{Host}/", UriKind.Absolute);
    }

    public static Uri BuildWithDocument(Uri markdownUri)
    {
        if (markdownUri is null)
        {
            throw new ArgumentNullException(nameof(markdownUri));
        }

        var encoded = Uri.EscapeDataString(markdownUri.AbsoluteUri);
        return new Uri($"{Scheme}://{Host}/?{UriParam}={encoded}", UriKind.Absolute);
    }

    public static bool TryParse(Uri routeUri, out Uri? markdownUri)
    {
        markdownUri = null;
        if (routeUri is null)
        {
            return false;
        }

        if (!routeUri.IsAbsoluteUri)
        {
            return false;
        }

        if (!string.Equals(routeUri.Scheme, Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(routeUri.Host, Host, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var query = ParseQuery(routeUri.Query);

        if (!query.TryGetValue(UriParam, out var encoded) || string.IsNullOrWhiteSpace(encoded))
        {
            return true;
        }

        var decoded = Uri.UnescapeDataString(encoded);

        if (Uri.TryCreate(decoded, UriKind.Absolute, out var doc))
        {
            markdownUri = doc;
            return true;
        }

        return false;
    }

    public static bool TryParseStrict(Uri routeUri, out Uri markdownUri)
    {
        markdownUri = null!;
        return TryParse(routeUri, out var doc) && doc is not null && (markdownUri = doc) is not null;
    }

    private static Dictionary<string, string> ParseQuery(string query)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(query))
        {
            return dict;
        }

        // "?a=1&b=2"
        var s = query;
        if (s.Length > 0 && s[0] == '?')
        {
            s = s.Substring(1);
        }

        if (string.IsNullOrEmpty(s))
        {
            return dict;
        }

        var pairs = s.Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var p in pairs)
        {
            var idx = p.IndexOf('=');
            if (idx < 0)
            {
                var k = Uri.UnescapeDataString(p);
                dict.TryAdd(k, "");

                continue;
            }

            var key = Uri.UnescapeDataString(p.Substring(0, idx));
            var val = Uri.UnescapeDataString(p.Substring(idx + 1));
            dict[key] = val;
        }

        return dict;
    }
}