namespace ZYC.Framework.Abstractions.Tab;

public class SimpleTabItemInfo
{
    public SimpleTabItemInfo(
        string host,
        string title,
        Type viewType,
        string icon,
        string? scheme = null,
        bool localization = true)
    {
        if (string.IsNullOrWhiteSpace(scheme))
        {
            scheme = ProductInfo.Scheme;
        }

        var builder = new UriBuilder
        {
            Scheme = scheme,
            Host = host
        };

        Uri = builder.Uri;

        Title = title;
        Icon = icon;
        ViewType = viewType;
        Localization = localization;
    }

    public string Scheme => Uri.Host;

    public string Host => Uri.Host;

    public string Title { get; }

    public string Icon { get; }

    public Type ViewType { get; }

    public bool Localization { get; }

    public Uri Uri { get; }
}