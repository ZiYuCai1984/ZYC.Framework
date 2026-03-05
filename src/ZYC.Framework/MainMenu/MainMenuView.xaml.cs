using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.MainMenu;

namespace ZYC.Framework.MainMenu;

[RegisterSingleInstance]
internal partial class MainMenuView : INotifyPropertyChanged
{
    public MainMenuView(
        IHamburgerMenuManager hamburgerMenuManager,
        IMainMenuManager mainMenuManager, 
        MainMenuConfig mainMenuConfig, 
        HamburgerMenuConfig hamburgerMenuConfig)
    {
        HamburgerMenuManager = hamburgerMenuManager;
        MainMenuManager = mainMenuManager;

        MainMenuConfig = mainMenuConfig;
        HamburgerMenuConfig = hamburgerMenuConfig;

        InitializeComponent();
    }

    private IHamburgerMenuManager HamburgerMenuManager { get; }
    private IMainMenuManager MainMenuManager { get; }

    public MainMenuConfig MainMenuConfig { get; }

    public HamburgerMenuConfig HamburgerMenuConfig { get; }

    public IMainMenuItem?[] MainMenuItems { get; set; } = [];

    public IMainMenuItem?[] HamburgerMenuItems { get; set; } = [];

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

        HamburgerMenuItems = HamburgerMenuManager.GetSortedItems();
        OnPropertyChanged(nameof(HamburgerMenuItems));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}