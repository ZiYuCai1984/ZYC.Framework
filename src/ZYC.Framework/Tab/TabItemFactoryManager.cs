using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Tab;

[RegisterSingleInstanceAs(typeof(ITabItemFactoryManager), typeof(ISimpleTabItemFactoryManager))]
internal partial class TabItemFactoryManager : ITabItemFactoryManager
{
    public TabItemFactoryManager(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    private IList<ITabItemFactory> TabItemFactories { get; } = new List<ITabItemFactory>();

    public void RegisterFactory<T>() where T : ITabItemFactory
    {
        TabItemFactories.Add(LifetimeScope.Resolve<T>());
    }

    public ITabItemFactory[] GetTabItemFactories()
    {
        return TabItemFactories
            .OrderByDescending(t => t.Priority)
            .ToArray();
    }
}