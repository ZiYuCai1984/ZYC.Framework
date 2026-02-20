namespace ZYC.Framework.Abstractions.QuickBar;

public interface IQuickBarItemsProvider
{
    IQuickBarItem[] GetTitleMenuItems();

    void AttachItem<T>() where T : IQuickBarItem;

    void AttachItem(IQuickBarItem item);

    void DetachItem<T>() where T : IQuickBarItem;

    void DetachItem(IQuickBarItem item);
}