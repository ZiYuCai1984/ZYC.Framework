namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides helpers for creating and comparing application URIs.
/// </summary>
public static class UriTools
{
    private static string Scheme => ProductInfo.Scheme;

    /// <summary>
    ///     Creates an application URI from a host.
    /// </summary>
    /// <param name="host">The host name.</param>
    /// <returns>The constructed URI.</returns>
    public static Uri CreateAppUri(string host)
    {
        var builder = new UriBuilder
        {
            Scheme = Scheme,
            Host = host
        };
        return builder.Uri;
    }

    public static Uri CreateUri(string scheme, string host = "", string path = "", string query = "")
    {
        var builder = new UriBuilder
        {
            Scheme = scheme,
            Host = host,
            Path = path,
            Query = query
        };
        return builder.Uri;
    }


    /// <summary>
    ///     Creates an application URI from a host and path.
    /// </summary>
    /// <param name="host">The host name.</param>
    /// <param name="path">The path segment.</param>
    /// <returns>The constructed URI.</returns>
    public static Uri CreateAppUri(string host, string path)
    {
        var builder = new UriBuilder
        {
            Scheme = Scheme,
            Host = host,
            Path = path
        };
        return builder.Uri;
    }

    /// <summary>
    ///     Creates an application URI from a host, path, and query.
    /// </summary>
    /// <param name="host">The host name.</param>
    /// <param name="path">The path segment.</param>
    /// <param name="query">The query string.</param>
    /// <returns>The constructed URI.</returns>
    public static Uri CreateAppUri(string host, string path, string query)
    {
        var builder = new UriBuilder
        {
            Scheme = Scheme,
            Host = host,
            Path = path,
            Query = query
        };
        return builder.Uri;
    }

    /// <summary>
    ///     Compares two URIs by their string representation.
    /// </summary>
    /// <param name="uri1">The first URI.</param>
    /// <param name="uri2">The second URI.</param>
    /// <returns>True if they match; otherwise, false.</returns>
    public static bool Equals(Uri uri1, Uri uri2)
    {
        var s1 = uri1.ToString();
        var s2 = uri2.ToString();

        return s1.Equals(s2);
    }

    /// <summary>
    ///     Determines whether a URI path starts with the specified path.
    /// </summary>
    /// <param name="uri">The URI to inspect.</param>
    /// <param name="path">The path prefix.</param>
    /// <returns>True if the path matches; otherwise, false.</returns>
    public static bool IsPathMatched(Uri uri, string path)
    {
        return uri.PathAndQuery.StartsWith(path);
    }

    /// <summary>
    ///     Normalizes a raw URI string with the application scheme.
    ///     If raw looks like a Windows path, it will be converted to a file:// URI.
    /// </summary>
    /// <param name="raw">The raw URI string.</param>
    /// <returns>The normalized URI string, or null if invalid.</returns>
    public static string? NormalizeUri(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        raw = raw.Trim();

        // 1) If it's already an absolute URI, keep it.
        if (Uri.TryCreate(raw, UriKind.Absolute, out var absolute))
        {
            return absolute.ToString();
        }

        // 2) Windows drive path -> file:///
        if (LooksLikeWindowsPath(raw))
        {
            // Normalize slashes so the resulting URI is canonical.
            var path = raw.Replace('\\', '/');

            // Handle "C:foo" (drive-relative) by making it "C:/foo"
            if (path.Length > 2 && path[2] != '/')
            {
                path = path.Insert(2, "/");
            }

            // file:///C:/...
            var fileUri = new Uri("file:///" + path);
            return fileUri.ToString();
        }

        // 3) UNC path -> file://server/share/...
        if (raw.StartsWith(@"\\", StringComparison.Ordinal))
        {
            var unc = raw.TrimStart('\\').Replace('\\', '/');
            if (Uri.TryCreate("file://" + unc, UriKind.Absolute, out var uncUri))
            {
                return uncUri.ToString();
            }

            return null;
        }

        // 4) Not a URI: apply app scheme unless "about:"
        if (!raw.Contains("://", StringComparison.Ordinal) &&
            !raw.StartsWith("about:", StringComparison.OrdinalIgnoreCase))
        {
            raw = $"{Scheme}://" + raw;
        }

        return Uri.TryCreate(raw, UriKind.Absolute, out var u) ? u.ToString() : null;
    }

    public static bool LooksLikeWindowsPath(string s)
    {
        return s.Length >= 2 && char.IsLetter(s[0]) && s[1] == ':';
    }
}