using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager;

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