using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.CLI.Abstractions;

namespace ZYC.Framework.Modules.CLI;

[RegisterSingleInstance]
internal class CLIMainMenuItem : MainMenuItem
{
    public CLIMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = CLIModuleConstants.DefaultTitle,
            Icon = CLIModuleConstants.Icon,
            Localization = false
        };

        Command = lifetimeScope.CreateNavigateCommand(CLIModuleConstants.Uri);
    }
}