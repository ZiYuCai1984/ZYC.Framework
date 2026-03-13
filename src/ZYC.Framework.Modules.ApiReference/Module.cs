using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ApiReference.Abstractions;


namespace ZYC.Framework.Modules.ApiReference;

internal class Module : ModuleBase
{
    public override string Icon => ApiReferenceModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<ApiReferenceTabItemFactory>();
        lifetimeScope.Resolve<IAboutMainMenuItemsProvider>()
            .RegisterSubItem<ApiReferenceMainMenuItem>();

        return Task.CompletedTask;
    }
}