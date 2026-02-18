using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ZYC.Automation.Abstractions.Config;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu;

[RegisterSingleInstance]
public partial class MainMenuView : INotifyPropertyChanged
{
    public MainMenuView(IMainMenuManager mainMenuManager, MainMenuConfig mainMenuConfig)
    {
        MainMenuManager = mainMenuManager;
        MainMenuConfig = mainMenuConfig;

        InitializeComponent();
    }

    private IMainMenuManager MainMenuManager { get; }

    public MainMenuConfig MainMenuConfig { get; }

    public IMainMenuItem?[] MainMenuItems { get; set; } = [];

    private bool FirstRending { get; set; } = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnMainMenuViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!FirstRending)
        {
            return;
        }

        FirstRending = false;


        MainMenuItems = MainMenuManager.GetSortedItems();
        OnPropertyChanged(nameof(MainMenuItems));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}