using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.BlazorDemo.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.BlazorDemo;

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