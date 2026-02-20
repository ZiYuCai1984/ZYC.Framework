using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.QuickBar;

namespace ZYC.Framework.QuickBar.BuildIn;

[RegisterSingleInstanceAs(typeof(ISimpleQuickBarItemsProvider))]
internal class SimpleQuickBarProvider : ISimpleQuickBarItemsProvider
{
    public SimpleQuickBarProvider(ILifetimeScope lifetimeScope, IEventAggregator eventAggregator)
    {
        LifetimeScope = lifetimeScope;
        EventAggregator = eventAggregator;
    }

    private ILifetimeScope LifetimeScope { get; }
    private IEventAggregator EventAggregator { get; }

    private IQuickBarItem[] QuickMenuTitleItems { get; set; } = [];

    public IQuickBarItem[] GetTitleMenuItems()
    {
        return QuickMenuTitleItems;
    }

    public void AttachItem<T>() where T : IQuickBarItem
    {
        var item = LifetimeScope.Resolve<T>();
        AttachItem(item);
    }

    public void AttachItem(IQuickBarItem item)
    {
        var list = new List<IQuickBarItem>(QuickMenuTitleItems) { item };
        QuickMenuTitleItems = list.ToArray();
        InvokeQuickMenuItemsChanged();
    }

    public void DetachItem<T>() where T : IQuickBarItem
    {
        throw new NotSupportedException();
    }

    public void DetachItem(IQuickBarItem item)
    {
        throw new NotSupportedException();
    }

    private void InvokeQuickMenuItemsChanged()
    {
        EventAggregator.Publish(new QuickMenuItemsChangedEvent());
    }
}