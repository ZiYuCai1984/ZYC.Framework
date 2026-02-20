namespace ZYC.Framework.Abstractions.QuickBar;

public interface IQuickBarManager
{
    void RegisterQuickMenuItemsProvider<T>() where T : IQuickBarItemsProvider;

    void UnregisterQuickMenuItemsProvider<T>() where T : IQuickBarItemsProvider;

    IQuickBarItem[] GetQuickMenuTitleItems();
}