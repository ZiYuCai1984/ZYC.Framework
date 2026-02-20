using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ModuleManager.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager;

[RegisterSingleInstance]
internal class NuGetModuleMainMenuItem : MainMenuItem
{
    public NuGetModuleMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = ModuleManagerModuleConstants.NuGet.Title,
            Icon = ModuleManagerModuleConstants.NuGet.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(ModuleManagerModuleConstants.NuGet.Uri);
    }
}