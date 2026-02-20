namespace ZYC.Framework.Abstractions.MainMenu;

public interface IMainMenuManager
{
    void RegisterItem(IMainMenuItem item);

    void RegisterItem<T>() where T : IMainMenuItem;

    IMainMenuItem[] GetItems();

    IMainMenuItem?[] GetSortedItems();
}