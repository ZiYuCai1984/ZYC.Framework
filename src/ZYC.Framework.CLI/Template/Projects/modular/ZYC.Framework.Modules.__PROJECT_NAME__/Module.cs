using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.__PROJECT_NAME__.Abstractions;

namespace ZYC.Framework.Modules.__PROJECT_NAME__;

internal class Module : ModuleBase
{
    public override string Icon => __PROJECT_SHORT_NAME__ModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        DebuggerTools.Attach();

        lifetimeScope.RegisterTabItemFactory<__PROJECT_SHORT_NAME__TabItemFactory>();
        lifetimeScope.Resolve<IExtensionsMainMenuItemsProvider>()
            .RegisterSubItem<__PROJECT_SHORT_NAME__MainMenuItem>();

        return Task.CompletedTask;
    }
}
