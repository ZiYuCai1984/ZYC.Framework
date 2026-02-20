using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;

namespace ZYC.Framework.MainMenu;

[RegisterSingleInstanceAs(typeof(IMainMenuManager))]
internal class MainMenuManager : IMainMenuManager
{
    public MainMenuManager(
        ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;

        RegisterItem<IFileMainMenuItemsProvider>();
        RegisterItem<IViewMainMenuItemsProvider>();
        RegisterItem<IToolsMainMenuItemsProvider>();
        RegisterItem<IExtensionsMainMenuItemsProvider>();
        RegisterItem<IAboutMainMenuItemsProvider>();
    }

    private ILifetimeScope LifetimeScope { get; }

    private IList<IMainMenuItem> Items { get; } = new List<IMainMenuItem>();


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
                .Select(SortSubItemsRecursively));

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

    private static IMainMenuItem SortSubItemsRecursively(IMainMenuItem item)
    {
        if (item.SubItems.Length == 0)
        {
            return item;
        }

        var groups = item.SubItems
            .GroupBy(x => x.Anchor)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .ToArray();

        var list = new List<IMainMenuItem?>();

        for (var i = 0; i < groups.Length; i++)
        {
            foreach (var sub in groups[i].OrderBy(x => x.Priority))
            {
                list.Add(SortSubItemsRecursively(sub));
            }

            if (i != groups.Length - 1)
            {
                list.Add(null);
            }
        }

        return new MainMenuItemWrapper(item, list.ToArray());
    }
}