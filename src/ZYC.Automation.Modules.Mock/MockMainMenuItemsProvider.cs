using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Core.Menu;
using ZYC.Automation.Modules.Mock.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Mock;

[RegisterSingleInstanceAs(typeof(IMockMainMenuItemsProvider))]
internal class MockMainMenuItemsProvider : MainMenuItemsProvider, IMockMainMenuItemsProvider, INotifyPropertyChanged
{
    public MockMainMenuItemsProvider(ILifetimeScope lifetimeScope, MockConfig mockConfig) : base(lifetimeScope)
    {
        MockConfig = mockConfig;
        Info = new MenuItemInfo
        {
            Title = "Mock",
            Localization = false,
            Priority = MainMenuPriority.About + 10
        };

        mockConfig.ObserveProperty(nameof(mockConfig.IsMainMenuVisible)).Subscribe(_ =>
        {
            OnPropertyChanged(nameof(IsHidden));
        });
    }

    private MockConfig MockConfig { get; }

    public override MenuItemInfo Info { get; }

    public override bool IsHidden => !MockConfig.IsMainMenuVisible;

    public void RegisterSubItem(MockTabItemInfo mockTabItemInfo)
    {
        var mockMainMenuItem =
            LifetimeScope.Resolve<MockMainMenuItem>(
                new TypedParameter(typeof(MockTabItemInfo), mockTabItemInfo));
        RegisterSubItem(mockMainMenuItem);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}