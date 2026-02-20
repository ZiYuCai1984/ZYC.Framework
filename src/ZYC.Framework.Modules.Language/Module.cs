using Autofac;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Language.Abstractions;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Language;

internal class Module : ModuleBase
{
    public override string Icon => LanguageModuleConstants.DefaultIcon;

    public override async Task RegisterAsync(ContainerBuilder builder)
    {
        await Task.CompletedTask;

        builder.RegisterAdapter<IConfig[], ILanguageResourcesConfig[]>(configs =>
            configs.OfType<ILanguageResourcesConfig>().ToArray());
    }

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<LanguageTabItemFactory>();

        var settingsMainMenuItem = lifetimeScope.Resolve<ISettingsMainMenuItemsProvider>();
        settingsMainMenuItem.RegisterSubItem<LanguageMainMenuItem>();

        return Task.CompletedTask;
    }
}