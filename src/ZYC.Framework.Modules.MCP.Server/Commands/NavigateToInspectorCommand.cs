using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server.Commands;

[RegisterSingleInstance]
internal class NavigateToInspectorCommand : CommandBase
{
    public NavigateToInspectorCommand(ITabManager tabManager)
    {
        TabManager = tabManager;
    }

    private ITabManager TabManager { get; }

    protected override void InternalExecute(object? parameter)
    {
        TabManager.NavigateAsync(MCPServerModuleConstants.Uri);
    }
}