using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.About.Abstractions;

namespace ZYC.Framework.Modules.About;

[RegisterSingleInstance]
internal class AboutMainMenuItem : MainMenuItem
{
    public AboutMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = AboutModuleConstants.Title,
            Icon = AboutModuleConstants.Icon,
            Anchor = AboutMainMenuAnchors.About,
            Localization = false,
            Priority = 10
        };

        Command = lifetimeScope.CreateNavigateCommand(AboutModuleConstants.Uri);
    }
}