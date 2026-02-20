using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Mock.Abstractions;

/// <summary>
///     Describes a mock tab item and its navigation metadata.
/// </summary>
public class MockTabItemInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MockTabItemInfo" /> class.
    /// </summary>
    /// <param name="viewType">The view type for the tab.</param>
    public MockTabItemInfo(Type viewType) : this(viewType,
        UriTools.CreateAppUri("mock", viewType.Name))
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MockTabItemInfo" /> class.
    /// </summary>
    /// <param name="viewType">The view type for the tab.</param>
    /// <param name="uri">The navigation URI for the tab.</param>
    public MockTabItemInfo(Type viewType, Uri uri)
    {
        ViewType = viewType;
        Uri = uri;
    }

    /// <summary>
    ///     Gets the view type for the tab.
    /// </summary>
    public Type ViewType { get; }

    /// <summary>
    ///     Gets the navigation URI for the tab.
    /// </summary>
    public Uri Uri { get; }

    /// <summary>
    ///     Gets the icon identifier for the tab.
    /// </summary>
    public string Icon => "BugOutline";

    /// <summary>
    ///     Gets the display title for the tab.
    /// </summary>
    public string Title => ViewType.Name;
}