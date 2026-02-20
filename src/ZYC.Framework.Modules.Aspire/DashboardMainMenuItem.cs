using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Aspire.Commands;

namespace ZYC.Framework.Modules.Aspire;

[RegisterSingleInstance]
internal class DashboardMainMenuItem : MainMenuItem
{
    public DashboardMainMenuItem(NavigateToDashboardCommand navigateToDashboardCommand)
    {
        Info = new MenuItemInfo
        {
            Title = "Dashboard",
            Icon = AspireModuleContansts.Icon
        };

        Command = navigateToDashboardCommand;
    }
}