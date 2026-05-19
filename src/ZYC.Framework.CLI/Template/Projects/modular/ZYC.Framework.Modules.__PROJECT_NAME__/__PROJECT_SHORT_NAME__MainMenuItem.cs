using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.__PROJECT_NAME__.Abstractions;

namespace ZYC.Framework.Modules.__PROJECT_NAME__;

[RegisterSingleInstance]
internal class __PROJECT_SHORT_NAME__MainMenuItem : MainMenuItem
{
    public __PROJECT_SHORT_NAME__MainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = __PROJECT_SHORT_NAME__ModuleConstants.Title,
            Icon = __PROJECT_SHORT_NAME__ModuleConstants.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(__PROJECT_SHORT_NAME__ModuleConstants.Uri);
    }
}
