using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ZYC.Automation.Abstractions.Config;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu;

[RegisterSingleInstance]
[Obsolete]
public sealed partial class MenuButtonView
{
    public MenuButtonView(IMainMenuManager mainMenuManager, MainMenuConfig mainMenuConfig)
    {
        MainMenuManager = mainMenuManager;
        MainMenuConfig = mainMenuConfig;

        InitializeComponent();
    }

    private IMainMenuManager MainMenuManager { get; }

    public MainMenuConfig MainMenuConfig { get; }

    public ObservableCollection<IMainMenuItem?> MainMenuItems { get; } = new();

    private bool FirstRending { get; set; } = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnMenuButtonViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!FirstRending)
        {
            return;
        }

        FirstRending = false;

        MainMenuItems.Clear();

        var items = MainMenuManager.GetSortedItems();
        foreach (var item in items)
        {
            MainMenuItems.Add(item);
        }

        OnPropertyChanged(nameof(MainMenuItems));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}