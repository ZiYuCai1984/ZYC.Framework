using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.BlazorDemo;

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