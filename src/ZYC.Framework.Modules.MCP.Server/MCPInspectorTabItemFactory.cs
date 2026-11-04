using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server;

[RegisterSingleInstance]
[TabItemRoute(Host = MCPServerModuleConstants.Host)]
internal class MCPInspectorTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<MCPInspectorTabItem>(new TypedParameter(
            typeof(TabReference), new TabReference(context.Uri)));
    }
}