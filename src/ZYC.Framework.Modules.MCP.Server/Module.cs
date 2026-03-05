using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server;

internal class Module : ModuleBase
{
    public override string Icon => Base64IconResources.MCP;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterToolsMainMenuItem<IMCPServerMainMenuItemsProvider>();

        return Task.CompletedTask;
    }
}