using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Menu;
using ZYC.Automation.Modules.ModuleManager.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.ModuleManager;

[RegisterSingleInstanceAs(typeof(IModuleManagerMainMenuItemsProvider))]
internal class ModuleManagerMainMenuItemsProvider : MainMenuItemsProvider, IModuleManagerMainMenuItemsProvider
{
    public ModuleManagerMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        RegisterSubItem<LocalModuleMainMenuItem>();
        RegisterSubItem<NuGetModuleMainMenuItem>();
    }

    public override MenuItemInfo Info { get; } = new()
    {
        Icon = ModuleManagerModuleConstants.Local.Icon,
        Title = "ModuleManager",
    };
}