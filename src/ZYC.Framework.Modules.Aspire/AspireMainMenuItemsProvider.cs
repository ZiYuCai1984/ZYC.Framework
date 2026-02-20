using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;
using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire;

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