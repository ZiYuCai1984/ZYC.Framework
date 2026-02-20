using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;
using ZYC.Framework.Modules.ModuleManager.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager;

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