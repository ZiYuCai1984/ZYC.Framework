using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Language.Abstractions;

namespace ZYC.Framework.Modules.Language;

[RegisterSingleInstance]
internal class LocalizationResourcesMainMenuItem : MainMenuItem
{
    public LocalizationResourcesMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = LanguageModuleConstants.LocalizationResources.Title,
            Icon = LanguageModuleConstants.LocalizationResources.Icon,
            Anchor = LanguageModuleConstants.Anchor
        };

        Command = lifetimeScope.CreateNavigateCommand(LanguageModuleConstants.LocalizationResources.Uri);
    }
}