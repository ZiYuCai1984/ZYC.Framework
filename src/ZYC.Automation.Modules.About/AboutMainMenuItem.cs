using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.About.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.About;

[RegisterSingleInstance]
internal class AboutMainMenuItem : MainMenuItem
{
    public AboutMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = AboutModuleConstants.Title,
            Icon = AboutModuleConstants.Icon,
            Anchor = AboutMainMenuAnchors.About
        };

        Command = lifetimeScope.CreateNavigateCommand(AboutModuleConstants.Uri);
    }
}