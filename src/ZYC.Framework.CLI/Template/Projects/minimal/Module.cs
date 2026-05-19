using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using __PROJECT_NAME__.UI;

namespace __PROJECT_NAME__;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        // Optional while developing the host:
        // ZYC.CoreToolkit.DebuggerTools.Attach();

        var simpleTabItemFactoryManager = lifetimeScope.Resolve<ISimpleTabItemFactoryManager>();
        simpleTabItemFactoryManager.Register(new SimpleTabItemFactoryInfo(typeof(__PROJECT_SHORT_NAME__View)));


        return Task.CompletedTask;
    }
}
