using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.TaskManager.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.TaskManager;

[RegisterSingleInstance]
[TabItemRoute(Host = TaskManagerModuleConstants.Host)]
internal class TaskManagerTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;

        return context.Resolve<TaskManagerTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}