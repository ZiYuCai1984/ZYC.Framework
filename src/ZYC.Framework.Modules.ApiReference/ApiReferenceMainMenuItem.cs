using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ApiReference.Abstractions;


namespace ZYC.Framework.Modules.ApiReference;

[RegisterSingleInstance]
internal class ApiReferenceMainMenuItem : MainMenuItem
{
    public ApiReferenceMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = ApiReferenceModuleConstants.Title,
            Icon = ApiReferenceModuleConstants.Icon,
            Anchor = AboutMainMenuAnchors.About
        };

        Command = lifetimeScope.CreateNavigateCommand(ApiReferenceModuleConstants.Uri);
    }
}