using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server;

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