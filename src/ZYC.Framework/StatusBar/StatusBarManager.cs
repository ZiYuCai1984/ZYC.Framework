using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.StatusBar;

namespace ZYC.Framework.StatusBar;

[RegisterSingleInstanceAs(typeof(IStatusBarManager))]
internal class StatusBarManager : IStatusBarManager
{
    public StatusBarManager(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;

        RegisterStatusBarItemsProvider<IDefaultStatusBarItemsProvider>();
    }

    private ILifetimeScope LifetimeScope { get; }

    private IList<IStatusBarItemsProvider> StatusBarItemsProviders { get; } = new List<IStatusBarItemsProvider>();


    public void RegisterStatusBarItemsProvider<T>() where T : IStatusBarItemsProvider
    {
        var provider = LifetimeScope.Resolve<T>();
        StatusBarItemsProviders.Add(provider);
    }

    public void UnregisterStatusBarItemsProvider<T>() where T : IStatusBarItemsProvider
    {
        throw new NotSupportedException();
    }

    public IStatusBarItem[] GetAllItems()
    {
        var statusBarItems = new List<IStatusBarItem>();

        var providers = StatusBarItemsProviders.ToArray();
        foreach (var provider in providers)
        {
            statusBarItems.AddRange(provider.GetStatusBarItems());
        }

        return statusBarItems.OrderBy(t => t.Order).ToArray();
    }
}