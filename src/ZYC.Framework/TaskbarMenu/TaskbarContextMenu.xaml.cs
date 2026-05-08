using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Hardcodet.Wpf.TaskbarNotification;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.TaskbarMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.TaskbarMenu.BuildIn;

namespace ZYC.Framework.TaskbarMenu;

[RegisterSingleInstanceAs(
    typeof(ITaskbarMenuManager), typeof(TaskbarContextMenu))]
internal partial class TaskbarContextMenu : ITaskbarMenuManager, IDisposable
{
    public TaskbarContextMenu(
        ILifetimeScope lifetimeScope,
        ShowWindowCommand showWindowCommand)
    {
        InheritanceBehavior = InheritanceBehavior.SkipToThemeNext;

        ShowWindowCommand = showWindowCommand;

        InitializeComponent();

        TaskbarIcon = new TaskbarIcon();
        TaskbarIcon.Icon = IconTools.CurrentProcessIcon;
        TaskbarIcon.ContextMenu = this;

        //!WARNING DoubleClickCommand is not work !!
        //TaskbarIcon.DoubleClickCommand = showWindowCommand;
        TaskbarIcon.TrayMouseDoubleClick += OnTaskbarIconTrayMouseDoubleClick;

        RegisterMenuItem(lifetimeScope.Resolve<FreezeWindowTaskbarItem>());
        RegisterMenuItem(lifetimeScope.Resolve<UnfreezeWindowTaskbarItem>());
        RegisterMenuItem(lifetimeScope.Resolve<ShowWindowTaskbarMenuItem>());
        RegisterMenuItem(lifetimeScope.Resolve<HideWindowTaskbarMenuItem>());
        RegisterMenuItem(lifetimeScope.Resolve<ExitProcessTaskbarItem>());


        AppContext.SetTaskbarIconReference(TaskbarIcon);
    }

    private ShowWindowCommand ShowWindowCommand { get; }

    private TaskbarIcon TaskbarIcon { get; }

    private IList<ITaskbarMenuItem> RegisteredTaskbarMenuItems { get; } = new List<ITaskbarMenuItem>();

    public ObservableCollection<ITaskbarMenuItem?> TaskbarMenuItems { get; } = new();

    public void Dispose()
    {
        TaskbarIcon.Dispose();
    }

    public void RegisterMenuItem(ITaskbarMenuItem menuItem)
    {
        RegisteredTaskbarMenuItems.Add(menuItem);
        RefreshTaskbarMenuItems();
    }

    private void RefreshTaskbarMenuItems()
    {
        TaskbarMenuItems.Clear();

        foreach (var item in GetSortedTaskbarMenuItems())
        {
            TaskbarMenuItems.Add(item);
        }
    }

    private ITaskbarMenuItem?[] GetSortedTaskbarMenuItems()
    {
        var groupedItems = RegisteredTaskbarMenuItems
            .GroupBy(t => t.Info.Anchor)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .ToArray();
        var list = new List<ITaskbarMenuItem?>();

        for (var i = 0; i < groupedItems.Length; i++)
        {
            list.AddRange(groupedItems[i]
                .OrderBy(t => t.Info.Priority)
                .Select(SortSubItemsRecursively));

            if (i != groupedItems.Length - 1)
            {
                list.Add(null);
            }
        }

        return list.ToArray();
    }

    private static ITaskbarMenuItem SortSubItemsRecursively(ITaskbarMenuItem item)
    {
        if (item.SubItems.Length == 0)
        {
            return item;
        }

        var subItems = item.SubItems
            .OrderBy(x => x.Info.Anchor, StringComparer.Ordinal)
            .ThenBy(x => x.Info.Priority)
            .Select(SortSubItemsRecursively)
            .ToArray();

        return new SortedTaskbarMenuItem(item, subItems);
    }

    private void OnTaskbarIconTrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        ShowWindowCommand.Execute(null);
    }

    private sealed class SortedTaskbarMenuItem : ITaskbarMenuItem
    {
        private readonly ITaskbarMenuItem _inner;

        public SortedTaskbarMenuItem(ITaskbarMenuItem inner, ITaskbarMenuItem[] subItems)
        {
            _inner = inner;
            SubItems = subItems;
        }

        public MenuItemInfo Info => _inner.Info;

        public ICommand Command => _inner.Command;

        public ITaskbarMenuItem[] SubItems { get; }
    }
}
