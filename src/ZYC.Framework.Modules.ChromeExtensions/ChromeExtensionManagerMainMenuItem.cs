using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;

namespace ZYC.Framework.Modules.ChromeExtensions;

[RegisterSingleInstance]
internal class ChromeExtensionManagerMainMenuItem : MainMenuItem
{
    public ChromeExtensionManagerMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Command = lifetimeScope.CreateNavigateCommand(ChromeExtensionsModuleConstants.Uri);
        Info = new MenuItemInfo
        {
            Title = ChromeExtensionsModuleConstants.Title,
            Icon = ChromeExtensionsModuleConstants.Icon
        };
    }
}
