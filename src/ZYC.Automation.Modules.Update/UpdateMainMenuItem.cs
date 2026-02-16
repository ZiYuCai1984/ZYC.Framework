using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Update.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Update;

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