using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.StatusBar;
using ZYC.Framework.Modules.TaskManager.UI;

namespace ZYC.Framework.Modules.TaskManager;

[RegisterSingleInstance]
internal class TaskManagerStatusBarItem : IStatusBarItem
{
    public TaskManagerStatusBarItem(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    public object View => LifetimeScope.Resolve<TaskManagerStatusBarItemView>();

    public StatusBarSection Section => StatusBarSection.Right;
}