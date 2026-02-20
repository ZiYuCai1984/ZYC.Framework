using System.Windows;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Settings.Abstractions;
using ZYC.Framework.Taskbar;

namespace ZYC.Framework;

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