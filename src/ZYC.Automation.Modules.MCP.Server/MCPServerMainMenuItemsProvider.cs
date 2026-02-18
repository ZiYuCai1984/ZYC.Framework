using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Menu;
using ZYC.Automation.Modules.MCP.Server.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server;

[RegisterSingleInstanceAs(typeof(IMCPServerMainMenuItemsProvider))]
internal class MCPServerMainMenuItemsProvider : MainMenuItemsProvider, IMCPServerMainMenuItemsProvider
{
    public MCPServerMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        RegisterSubItem<StartServerMainMenuItem>();
        RegisterSubItem<StopServerMainMenuItem>();
    }

    public override MenuItemInfo Info => new()
    {
        Title = MCPServerModuleConstants.Title,
        Icon = MCPServerModuleConstants.Icon,
        Localization = false
    };
}