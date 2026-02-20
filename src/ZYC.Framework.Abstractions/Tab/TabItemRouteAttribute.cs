namespace ZYC.Framework.Abstractions.Tab;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class TabItemRouteAttribute : Attribute
{
    public string? Scheme { get; init; }

    public string? Host { get; init; }

    public string? Path { get; init; }

    public PathMatchMode PathMatch { get; init; } = PathMatchMode.Exact;

    public bool MightMatch(Uri uri)
    {
        if (Scheme is not null &&
            !string.Equals(uri.Scheme, Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (Host is not null &&
            !string.Equals(uri.Host, Host, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

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

public enum PathMatchMode
{
    Exact,
    Prefix
}