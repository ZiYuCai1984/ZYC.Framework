using System.Collections.ObjectModel;
using System.Windows;
using Autofac;
using Hardcodet.Wpf.TaskbarNotification;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.TaskbarMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Taskbar.BuildIn;

namespace ZYC.Framework.Taskbar;

[RegisterSingleInstanceAs(
    typeof(ITaskbarMenuManager), typeof(TaskbarContextMenu))]
internal partial class TaskbarContextMenu : ITaskbarMenuManager, IDisposable
{
    public TaskbarContextMenu(
        ILifetimeScope lifetimeScope,
        ShowWindowCommand showWindowCommand)
    {
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

    public ObservableCollection<ITaskbarMenuItem> TaskbarMenuItems { get; } = new();

    public void Dispose()
    {
        TaskbarIcon.Dispose();
    }

    public void RegisterMenuItem(ITaskbarMenuItem menuItem)
    {
        TaskbarMenuItems.Add(menuItem);
    }

    private void OnTaskbarIconTrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        ShowWindowCommand.Execute(null);
    }
}