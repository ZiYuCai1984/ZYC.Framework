using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ModuleManager.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager;

[RegisterSingleInstance]
internal class LocalModuleMainMenuItem : MainMenuItem
{
    public LocalModuleMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = ModuleManagerModuleConstants.Local.Title,
            Icon = ModuleManagerModuleConstants.Local.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(ModuleManagerModuleConstants.Local.Uri);
    }
}