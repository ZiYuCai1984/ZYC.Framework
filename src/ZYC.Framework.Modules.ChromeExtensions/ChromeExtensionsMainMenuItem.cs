using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;


namespace ZYC.Framework.Modules.ChromeExtensions;

[RegisterSingleInstance]
internal class ChromeExtensionsMainMenuItem : MainMenuItem
{
    public ChromeExtensionsMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = ChromeExtensionsModuleConstants.Title,
            Icon = ChromeExtensionsModuleConstants.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(ChromeExtensionsModuleConstants.Uri);
    }
}