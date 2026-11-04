using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.MCP.Server.Abstractions;
using ZYC.Framework.Modules.MCP.Server.UI;

namespace ZYC.Framework.Modules.MCP.Server;

[Register]
[ConstantsSource(typeof(MCPServerModuleConstants))]
internal class MCPInspectorTabItem : TabItemInstanceBase<MCPInspectorView>
{
    public MCPInspectorTabItem(ILifetimeScope lifetimeScope, TabReference tabReference) : base(lifetimeScope,
        tabReference)
    {
    }

    public override bool Localization => false;
}