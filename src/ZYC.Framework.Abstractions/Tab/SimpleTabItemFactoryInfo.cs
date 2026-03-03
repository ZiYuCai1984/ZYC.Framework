namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Metadata used by the factory to create and register tab items and their associated menu entries.
/// </summary>
public class SimpleTabItemFactoryInfo
{
    /// <summary>
    ///     Initializes a new instance with full menu and tab information.
    /// </summary>
    public SimpleTabItemFactoryInfo(
        SimpleMainMenuItemInfo mainMenuItemInfo,
        SimpleTabItemInfo tabItemInfo,
        bool addQuickBarItem = true,
        bool isSingle = true)
    {
        IsSingle = isSingle;
        MainMenuItemInfo = mainMenuItemInfo;
        TabItemInfo = tabItemInfo;
        AddQuickBarItem = addQuickBarItem;
    }

    /// <summary>
    ///     Initializes a new instance by providing host, title, and view type.
    ///     Internal menu and tab info objects will be created automatically.
    /// </summary>
    public SimpleTabItemFactoryInfo(
        string host,
        string title,
        Type viewType,
        bool addQuickBarItem = true,
        string icon = "PuzzlePlusOutline",
        bool isSingle = true) : this(new SimpleMainMenuItemInfo(title, icon),
        new SimpleTabItemInfo(host, title, viewType, icon), addQuickBarItem, isSingle)
    {
    }

    /// <summary>
    ///     Initializes a new instance where the host name is also used as the title.
    /// </summary>
    public SimpleTabItemFactoryInfo(
        string host,
        Type viewType,
        bool addQuickBarItem = true,
        string icon = "PuzzlePlusOutline",
        bool isSingle = true) : this(host, host, viewType, addQuickBarItem, icon, isSingle)
    {
    }

    /// <summary>
    ///     Initializes a new instance using the type name of <paramref name="viewType" /> as the host.
    /// </summary>
    public SimpleTabItemFactoryInfo(
        Type viewType,
        bool addQuickBarItem = true,
        string icon = "PuzzlePlusOutline",
        bool isSingle = true) : this(viewType.Name, viewType, addQuickBarItem, icon, isSingle)
    {
    }

    /// <summary>Gets the associated menu item configuration.</summary>
    public SimpleMainMenuItemInfo MainMenuItemInfo { get; }

    /// <summary>Gets the associated tab item configuration.</summary>
    public SimpleTabItemInfo TabItemInfo { get; }

    /// <summary>Gets a value indicating whether only a single instance of this tab should exist.</summary>
    public bool IsSingle { get; }

    /// <summary>Gets a value indicating whether to add this item to the quick access bar.</summary>
    public bool AddQuickBarItem { get; }
}