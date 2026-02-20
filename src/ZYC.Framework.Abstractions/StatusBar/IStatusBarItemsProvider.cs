namespace ZYC.Framework.Abstractions.StatusBar;

public interface IStatusBarItemsProvider
{
    IStatusBarItem[] GetStatusBarItems();

    void RegisterItem<T>() where T : IStatusBarItem;

    void RegisterItem(IStatusBarItem item);

    void UnregisterItem<T>() where T : IStatusBarItem;

    void UnregisterItem(IStatusBarItem item);
}