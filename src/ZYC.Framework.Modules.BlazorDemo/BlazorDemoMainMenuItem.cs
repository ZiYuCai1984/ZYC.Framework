using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.BlazorDemo.Abstractions;

namespace ZYC.Framework.Modules.BlazorDemo;

[RegisterSingleInstance]
internal class BlazorDemoMainMenuItem : MainMenuItem
{
    public BlazorDemoMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = BlazorDemoModuleConstants.Title,
            Icon = BlazorDemoModuleConstants.Icon,
            Localization = false
        };

        Command = lifetimeScope.CreateNavigateCommand(BlazorDemoModuleConstants.Uri);
    }
}