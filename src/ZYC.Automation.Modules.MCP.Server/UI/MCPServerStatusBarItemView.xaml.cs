using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Abstractions.Notification.Toast;
using ZYC.Automation.Modules.MCP.Server.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server.UI;

[RegisterSingleInstanceAs(typeof(MCPServerStatusBarItemView), typeof(IMCPServerManager))]
internal partial class MCPServerStatusBarItemView : INotifyPropertyChanged
{
    public MCPServerStatusBarItemView(
        IToastManager toastManager,
        ILifetimeScope lifetimeScope,
        MCPServerConfig mcpServerConfig)
    {
        ToastManager = toastManager;
        LifetimeScope = lifetimeScope;
        MCPServerConfig = mcpServerConfig;

        InitializeComponent();
    }

    private IToastManager ToastManager { get; }

    private ILifetimeScope LifetimeScope { get; }

    private MCPServerConfig MCPServerConfig { get; }

    private bool FirstRending { get; set; } = true;

    public IMainMenuItem[] MainMenuItems { get; set; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    private async void OnMCPServerStatusBarItemViewLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!FirstRending)
            {
                return;
            }

            FirstRending = false;
            MainMenuItems = LifetimeScope.Resolve<IMCPServerMainMenuItemsProvider>().SubItems;
            OnPropertyChanged(nameof(MainMenuItems));

            if (MCPServerConfig.AutoStart)
            {
                await StartServerAsync();
            }
        }
        catch
        {
            //ignore
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}