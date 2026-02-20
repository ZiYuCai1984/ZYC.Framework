namespace ZYC.Framework.Abstractions.StatusBar;

public interface IStatusBarManager
{
    void RegisterStatusBarItemsProvider<T>() where T : IStatusBarItemsProvider;

    void UnregisterStatusBarItemsProvider<T>() where T : IStatusBarItemsProvider;

    IStatusBarItem[] GetAllItems();

    IStatusBarItem[] GetLeftItems()
    {
        return GetAllItems().Where(t => t.Section == StatusBarSection.Left).ToArray();
    }

    IStatusBarItem[] GetRightItems()
    {
        return GetAllItems().Where(t => t.Section == StatusBarSection.Right).ToArray();
    }
}