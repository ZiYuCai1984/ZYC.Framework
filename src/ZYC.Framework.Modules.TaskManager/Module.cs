using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager;

internal class Module : ModuleBase
{
    public override string Icon => TaskManagerModuleConstants.Icon;

    public override async Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        var taskManager = lifetimeScope.Resolve<ITaskManager>();

        //TODO-zyc If the TaskManager module is disabled, InitializeAsync will not be called here; consequently, tasks will never start (even though they can still be resolved)...
        //This seems quite reasonable—if the TaskManager is disabled, tasks shouldn't start.

        await taskManager.InitializeAsync(CancellationToken.None);

        lifetimeScope.RegisterToolsMainMenuItem<TaskManagerMainMenuItem>();

        var tabItemFactoryManager = lifetimeScope.Resolve<ITabItemFactoryManager>();
        tabItemFactoryManager.RegisterFactory<TaskManagerTabItemFactory>();


        lifetimeScope.RegisterDefaultStatusBarItem<TaskManagerStatusBarItem>();
    }
}