namespace ZYC.Framework.Abstractions.Tab;

public class SimpleTabItemFactoryInfo
{
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

    public SimpleTabItemFactoryInfo(
        string host,
        Type viewType,
        bool addQuickBarItem = true,
        string icon = "PuzzlePlusOutline",
        bool isSingle = true) : this(host, host, viewType, addQuickBarItem, icon, isSingle)
    {
    }

    public SimpleTabItemFactoryInfo(
        Type viewType,
        bool addQuickBarItem = true,
        string icon = "PuzzlePlusOutline",
        bool isSingle = true) : this(viewType.Name, viewType, addQuickBarItem, icon, isSingle)
    {
    }

    public SimpleMainMenuItemInfo MainMenuItemInfo { get; }

    public SimpleTabItemInfo TabItemInfo { get; }

    public bool IsSingle { get; }

    public bool AddQuickBarItem { get; }
}