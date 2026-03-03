namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Defines a routing rule for a tab item factory based on URI components.
///     Multiple attributes can be applied to support multiple routes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class TabItemRouteAttribute : Attribute
{
    /// <summary>Gets or sets the required URI scheme (e.g., "http", "app"). If null, matches any scheme.</summary>
    public string? Scheme { get; init; }

    /// <summary>Gets or sets the required URI host. If null, matches any host.</summary>
    public string? Host { get; init; }

    /// <summary>Gets or sets the required URI path.</summary>
    public string? Path { get; init; }

    /// <summary>Gets or sets the matching strategy for the path. Defaults to <see cref="PathMatchMode.Exact" />.</summary>
    public PathMatchMode PathMatch { get; init; } = PathMatchMode.Exact;

    /// <summary>
    ///     Validates if the provided URI matches the criteria defined in this attribute.
    /// </summary>
    /// <param name="uri">The URI to evaluate.</param>
    /// <returns>True if the URI satisfies all specified components (Scheme, Host, Path).</returns>
    public bool MightMatch(Uri uri)
    {
        // Validate Scheme if specified
        if (Scheme is not null &&
            !string.Equals(uri.Scheme, Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Validate Host if specified
        if (Host is not null &&
            !string.Equals(uri.Host, Host, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // If no Path is specified, Scheme/Host match is sufficient
        if (Path is null)
        {
            return true;
        }

        var uriPath = NormalizePath(uri.AbsolutePath);
        var attrPath = NormalizePath(Path);

        return PathMatch switch
        {
            PathMatchMode.Exact => string.Equals(uriPath, attrPath, StringComparison.OrdinalIgnoreCase),
            PathMatchMode.Prefix => uriPath.StartsWith(attrPath, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    /// <summary>
    ///     Standardizes path strings by ensuring leading slashes and removing trailing slashes.
    /// </summary>
    private static string NormalizePath(string p)
    {
        p = p.Trim();
        if (!p.StartsWith('/'))
        {
            p = "/" + p;
        }

        if (p.Length > 1 && p.EndsWith('/'))
        {
            p = p.TrimEnd('/');
        }

        return p;
    }
}

/// <summary>
///     Specifies how the URI path should be compared.
/// </summary>
public enum PathMatchMode
{
    /// <summary>The paths must be identical (case-insensitive).</summary>
    Exact,

    /// <summary>The URI path must start with the defined path segment.</summary>
    Prefix
}