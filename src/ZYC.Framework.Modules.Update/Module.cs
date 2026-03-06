using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Update.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions.Commands;

namespace ZYC.Framework.Modules.Update;

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

        lifetimeScope.SubscribeEvent<TabManagerRestoreCompleted>(_ =>
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