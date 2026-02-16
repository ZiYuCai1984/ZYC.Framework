using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Menu;
using ZYC.Automation.Modules.Mock.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Mock;

[RegisterSingleInstanceAs(typeof(IMockMainMenuItemsProvider))]
internal class MockMainMenuItemsProvider : MainMenuItemsProvider, IMockMainMenuItemsProvider
{
    public MockMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Mock",
            Localization = false,
            Priority = MainMenuPriority.About + 10
        };
    }

    public override MenuItemInfo Info { get; }

    public void RegisterSubItem(MockTabItemInfo mockTabItemInfo)
    {
        var mockMainMenuItem =
            LifetimeScope.Resolve<MockMainMenuItem>(
                new TypedParameter(typeof(MockTabItemInfo), mockTabItemInfo));
        RegisterSubItem(mockMainMenuItem);
    }
}