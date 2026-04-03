using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Workspace;

[RegisterSingleInstanceAs(typeof(IWorkspaceContextMenuManager))]
internal class WorkspaceContextMenuManager : IWorkspaceContextMenuManager
{
    //TODO-zyc WorkspaceContextMenuManager
    public WorkspaceContextMenuManager(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    private IList<IWorkspaceMenuItem> WorkspaceMenuItems { get; } = new List<IWorkspaceMenuItem>();

    public void RegisterItem(IWorkspaceMenuItem item)
    {
        WorkspaceMenuItems.Add(item);
    }

    public void RegisterItem<T>() where T : IWorkspaceMenuItem
    {
        WorkspaceMenuItems.Add(LifetimeScope.Resolve<T>());
    }

    public IWorkspaceMenuItem[] GetItems()
    {
        return WorkspaceMenuItems.ToArray();
    }
}