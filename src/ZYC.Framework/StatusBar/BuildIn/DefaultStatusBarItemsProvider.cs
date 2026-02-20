using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.StatusBar;

namespace ZYC.Framework.StatusBar.BuildIn;

[RegisterSingleInstanceAs(typeof(IDefaultStatusBarItemsProvider))]
public class DefaultStatusBarItemsProvider : IDefaultStatusBarItemsProvider
{
    public DefaultStatusBarItemsProvider(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    private IList<IStatusBarItem> StatusBarItem { get; } = new List<IStatusBarItem>();


    public IStatusBarItem[] GetStatusBarItems()
    {
        return StatusBarItem.ToArray();
    }

    public void RegisterItem<T>() where T : IStatusBarItem
    {
        RegisterItem(LifetimeScope.Resolve<T>());
    }

    public void RegisterItem(IStatusBarItem item)
    {
        StatusBarItem.Add(item);
    }

    public void UnregisterItem<T>() where T : IStatusBarItem
    {
        throw new NotSupportedException();
    }

    public void UnregisterItem(IStatusBarItem item)
    {
        throw new NotSupportedException();
    }
}