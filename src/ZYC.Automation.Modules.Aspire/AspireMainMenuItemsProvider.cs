using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Menu;
using ZYC.Automation.Modules.Aspire.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Aspire;

[RegisterSingleInstanceAs(typeof(IAspireMainMenuItemsProvider))]
internal class AspireMainMenuItemsProvider : MainMenuItemsProvider, IAspireMainMenuItemsProvider
{
    public AspireMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        RegisterSubItem<DashboardMainMenuItem>();
        RegisterSubItem<StartServerMainMenuItem>();
        RegisterSubItem<StopServerMainMenuItem>();
        RegisterSubItem<SetAspireBinarySourceMainMenuItem>();
        RegisterSubItem<DownloadAspireToolsMainMenuItem>();
    }

    public override MenuItemInfo Info => new()
    {
        Title = AspireModuleContansts.Title,
        Icon = AspireModuleContansts.Icon,
        Localization = false
    };
}