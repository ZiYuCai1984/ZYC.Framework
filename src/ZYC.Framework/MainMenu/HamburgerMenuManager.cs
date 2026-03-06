using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.MainMenu.BuildIn;

namespace ZYC.Framework.MainMenu;

[RegisterSingleInstanceAs(typeof(IHamburgerMenuManager))]
internal class HamburgerMenuManager : IHamburgerMenuManager
{
    private ILifetimeScope LifetimeScope { get; }

    public HamburgerMenuManager(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;

        RegisterItem<RestartMainMenuItem>();
        RegisterItem<ExitMainMenuItem>();
    }

    public void RegisterItem(IMainMenuItem item)
    {
        Items.Add(item);
    }

    public void RegisterItem<T>() where T : IMainMenuItem
    {
        RegisterItem(LifetimeScope.Resolve<T>());
    }

    public IMainMenuItem[] GetItems()
    {
        return Items.ToArray();
    }


    public IMainMenuItem?[] GetSortedItems()
    {
        var groupedItems = GetGroupedMainMenuItems();
        var list = new List<IMainMenuItem?>();

        for (var i = 0; i < groupedItems.Length; i++)
        {
            list.AddRange(groupedItems[i]
                .OrderBy(t => t.Priority)
                .Select(MainMenuManager.SortSubItemsRecursively));

            if (i != groupedItems.Length - 1)
            {
                list.Add(null);
            }
        }

        return list.ToArray();
    }

    private IGrouping<string, IMainMenuItem>[] GetGroupedMainMenuItems()
    {
        var items = GetItems();
        var groupedItems =
            items.GroupBy(t => t.Anchor)
                .OrderBy(g => g.Key, StringComparer.Ordinal)
                .ToArray();

        return groupedItems;
    }


    private IList<IMainMenuItem> Items { get; } = new List<IMainMenuItem>();

}
