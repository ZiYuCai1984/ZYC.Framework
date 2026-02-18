using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.TaskManager.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Automation.Modules.TaskManager;

internal class Module : ModuleBase
{
    public override string Icon => TaskManagerModuleConstants.Icon;

    public override async Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        var taskManager = lifetimeScope.Resolve<ITaskManager>();
        await taskManager.InitializeAsync(CancellationToken.None);

        lifetimeScope.RegisterToolsMainMenuItem<TaskManagerMainMenuItem>();

        var tabItemFactoryManager = lifetimeScope.Resolve<ITabItemFactoryManager>();
        tabItemFactoryManager.RegisterFactory<TaskManagerTabItemFactory>();


        lifetimeScope.RegisterDefaultStatucBarItem<TaskManagerStatusBarItem>();
    }
}