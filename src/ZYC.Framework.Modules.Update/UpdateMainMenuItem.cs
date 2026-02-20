using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update;

[RegisterSingleInstance]
internal class UpdateMainMenuItem : MainMenuItem
{
    public UpdateMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = UpdateModuleConstants.Title,
            Icon = UpdateModuleConstants.Icon,
            Anchor = AboutMainMenuAnchors.Update
        };

        Command = lifetimeScope.CreateNavigateCommand(UpdateModuleConstants.Uri);
    }
}