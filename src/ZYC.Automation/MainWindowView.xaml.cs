using System.Windows;
using Autofac;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Abstractions.Event;
using ZYC.Automation.Abstractions.Notification.Toast;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Settings.Abstractions;
using ZYC.Automation.Taskbar;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation;

[RegisterSingleInstanceAs(typeof(MainWindowView), typeof(IRootGrid))]
internal partial class MainWindowView : IRootGrid
{
    public MainWindowView(
        ILifetimeScope lifetimeScope,
        TaskbarContextMenu taskbarContextMenu)
    {
        LifetimeScope = lifetimeScope;
        TaskbarContextMenu = taskbarContextMenu;

        InitializeComponent();
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private ILifetimeScope LifetimeScope { get; }

    private TaskbarContextMenu TaskbarContextMenu { get; }

    private bool FirstRending { get; set; } = true;

    public object GetRootGrid()
    {
        return RootGrid;
    }


    private void OnMainWindowViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!FirstRending)
        {
            return;
        }

        FirstRending = false;

        if (!LifetimeScope.TryResolve<ISettingsManager>(out _))
        {
            LifetimeScope.Resolve<IToastManager>().PromptMessage(
                ToastMessage.Warn("Missing Settings module,some features don't work properly !!"));
        }


        LifetimeScope.PublishEvent(new MainWindowLoadedEvent());
    }
}