using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;

namespace ZYC.Framework.Modules.ChromeExtensions;

internal class Module : ModuleBase
{
    public override string Icon => ChromeExtensionsModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<ChromeExtensionManagerTabItemFactory>();
        lifetimeScope.RegisterExtensionsMainMenuItem<ChromeExtensionManagerMainMenuItem>();

        return Task.CompletedTask;
    }
}