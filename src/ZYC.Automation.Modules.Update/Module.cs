using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Autofac;
using ZYC.Automation.Abstractions.Event;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Update.Abstractions;
using ZYC.Automation.Modules.Update.Abstractions.Commands;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Automation.Modules.Update;

internal class Module : ModuleBase
{
    public override string Icon => UpdateModuleConstants.Icon;

    private CompositeDisposable CompositeDisposable { get; } = new();

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<UpdateTabItemFactory>();
        lifetimeScope.RegisterAboutMainMenuItem<UpdateMainMenuItem>();

        return Task.CompletedTask;
    }

    public override async Task AfterLoadedAsync(ILifetimeScope lifetimeScope)
    {
        await Task.CompletedTask;

        lifetimeScope.SubscribeEvent<MainWindowLoadedEvent>(_ =>
        {
            var updateConfig = lifetimeScope.Resolve<UpdateConfig>();
            if (!updateConfig.CheckAtStartup)
            {
                return;
            }

            var checkUpdateCommand = lifetimeScope.Resolve<ICheckUpdateCommand>();
            checkUpdateCommand.Execute();
        }).DisposeWith(CompositeDisposable);
    }
}