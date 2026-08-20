using Autofac;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Commands;

namespace ZYC.Framework.Workspace;

[RegisterSingleInstanceAs(typeof(IWorkspaceContextMenuManager))]
internal class WorkspaceContextMenuManager : IWorkspaceContextMenuManager
{
    public WorkspaceContextMenuManager(
        ILifetimeScope lifetimeScope,
        IWorkspaceMenuManager workspaceMenuManager,
        HideNavigationBarCommand hideNavigationBarCommand,
        ShowNavigationBarCommand showNavigationBarCommand)
    {
        LifetimeScope = lifetimeScope;

        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "Current Workspace",
                null,
                null,
                subItems: workspaceMenuManager.GetItems()));

        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "Hide Navigation Bar",
                hideNavigationBarCommand,
                null));

        WorkspaceMenuItems.Add(
            new WorkspaceMenuItem(
                "Show Navigation Bar",
                showNavigationBarCommand,
                null));
    }

    private ILifetimeScope LifetimeScope { get; }

    private IList<IWorkspaceMenuItem> WorkspaceMenuItems { get; } = new List<IWorkspaceMenuItem>();

    public void RegisterItem(IWorkspaceMenuItem item)
    {
        WorkspaceMenuItems.Add(item);
    }

    public void RegisterItem<T>() where T : IWorkspaceMenuItem
    {
        WorkspaceMenuItems.Add(LifetimeScope.Resolve<T>());
    }

    public IWorkspaceMenuItem[] GetItems()
    {
        return WorkspaceMenuItems
            .GroupBy(t => t.Anchor)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .SelectMany(g => g
                .OrderBy(t => t.Priority)
                .Select(SortSubItemsRecursively))
            .ToArray();
    }

    private static IWorkspaceMenuItem SortSubItemsRecursively(IWorkspaceMenuItem item)
    {
        if (item.SubItems.Length == 0)
        {
            return item;
        }

        var subItems = item.SubItems
            .GroupBy(t => t.Anchor)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .SelectMany(g => g
                .OrderBy(t => t.Priority)
                .Select(SortSubItemsRecursively))
            .ToArray();

        return new SortedWorkspaceMenuItem(item, subItems);
    }

    private sealed class SortedWorkspaceMenuItem : IWorkspaceMenuItem
    {
        private readonly IWorkspaceMenuItem _inner;

        public SortedWorkspaceMenuItem(IWorkspaceMenuItem inner, IWorkspaceMenuItem[] subItems)
        {
            _inner = inner;
            SubItems = subItems;
        }

        public string Title => _inner.Title;

        public ICommand? Command => _inner.Command;

        public IWorkspaceMenuItem[] SubItems { get; }

        public string? Icon => _inner.Icon;

        public string Anchor => _inner.Anchor;

        public int Priority => _inner.Priority;

        public bool Localization => _inner.Localization;
    }
}