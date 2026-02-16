using Autofac;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Core;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Automation.Modules.BlazorDemo;

internal class Module : ModuleBase
{
    public override string Icon => Base64IconResources.Blazor;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<BlazorDemoTabItemFactory>();
        lifetimeScope.RegisterToolsMainMenuItem<BlazorDemoMainMenuItem>();

        return Task.CompletedTask;
    }
}