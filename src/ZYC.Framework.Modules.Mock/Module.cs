using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Mock.Abstractions;
using ZYC.Framework.Modules.Mock.UI;

namespace ZYC.Framework.Modules.Mock;

internal class Module : ModuleBase
{
    public override string Icon => "🚀";

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<IMockTabItemFactory>();

        lifetimeScope.RegisterRootMainMenuItem<IMockMainMenuItemsProvider>();

        var mockTabItemFactory = lifetimeScope.Resolve<IMockTabItemFactory>();


        mockTabItemFactory.RegisterMockTabItem(
            new MockTabItemInfo(typeof(NotRegisteredExceptionView)));
        mockTabItemFactory.RegisterMockTabItem(
            new MockTabItemInfo(typeof(TestWorkspaceView)));
        mockTabItemFactory.RegisterMockTabItem(
            new MockTabItemInfo(typeof(TestNotificationView)));
        mockTabItemFactory.RegisterMockTabItem(
            new MockTabItemInfo(typeof(TestTaskManagerView)));
        mockTabItemFactory.RegisterMockTabItem(
            new MockTabItemInfo(typeof(TestCLIView)));

        if (lifetimeScope.TryResolve<ICommandlineResourcesProvider>(
                out var commandlineResourcesProvider))
        {
            commandlineResourcesProvider.Register(new CommandlineServiceOptions
            {
                Name = "mock",
                Command = "echo Hello World"
            });
        }

        return Task.CompletedTask;
    }
}