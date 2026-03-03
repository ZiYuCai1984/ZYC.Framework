namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Contains detailed information for a tab item, including navigation URI and view mapping.
/// </summary>
public class SimpleTabItemInfo
{
    /// <summary>
    ///     Initializes a new instance and constructs the navigation <see cref="Uri" />.
    /// </summary>
    /// <param name="host">The host part of the URI.</param>
    /// <param name="title">The display title.</param>
    /// <param name="viewType">The type of the view associated with this tab.</param>
    /// <param name="icon">The icon identifier.</param>
    /// <param name="scheme">The URI scheme. If null, uses <see cref="ProductInfo.Scheme" />.</param>
    /// <param name="localization">Whether to enable localization.</param>
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

    /// <summary>Gets the URI scheme (e.g., "app", "https").</summary>
    public string Scheme => Uri.Scheme;

    /// <summary>Gets the host defined in the URI.</summary>
    public string Host => Uri.Host;

    /// <summary>Gets the display title.</summary>
    public string Title { get; }

    /// <summary>Gets the icon identifier.</summary>
    public string Icon { get; }

    /// <summary>Gets the UI component type to be instantiated.</summary>
    public Type ViewType { get; }

    /// <summary>Gets a value indicating whether localization is enabled.</summary>
    public bool Localization { get; }

    /// <summary>Gets the unique navigation URI for this tab item.</summary>
    public Uri Uri { get; }
}