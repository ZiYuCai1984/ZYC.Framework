using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.CLI.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.CLI;

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