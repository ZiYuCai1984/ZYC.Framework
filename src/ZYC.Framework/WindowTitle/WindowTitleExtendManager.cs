using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.WindowTitle;

namespace ZYC.Framework.WindowTitle;

[RegisterSingleInstanceAs(typeof(IWindowTitleExtendManager))]
internal class WindowTitleExtendManager : IWindowTitleExtendManager
{
    public WindowTitleExtendManager(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    private IList<IWindowTitleExtendItem> Items { get; } = new List<IWindowTitleExtendItem>();

    public void RegisterItem(IWindowTitleExtendItem item)
    {
        Items.Add(item);
    }

    public void RegisterItem<T>() where T : IWindowTitleExtendItem
    {
        var item = LifetimeScope.Resolve<T>();
        Items.Add(item);
    }

    public IWindowTitleExtendItem[] GetItems()
    {
        return Items.ToArray();
    }
}