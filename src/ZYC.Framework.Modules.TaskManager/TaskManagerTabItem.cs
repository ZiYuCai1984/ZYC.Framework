using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.TaskManager.UI;

namespace ZYC.Framework.Modules.TaskManager;

[Register]
[ConstantsSource(typeof(TaskManagerModuleConstants))]
internal class TaskManagerTabItem : TabItemInstanceBase<TaskManagerView>
{
    public TaskManagerTabItem(ILifetimeScope lifetimeScope, TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }

    public override void Dispose()
    {
        //ignore
    }
}