using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Tab.BuildIn;

[Register]
internal class SimpleTabItem : ITabItemInstance
{
    private object? _view;

    public SimpleTabItem(ILifetimeScope lifetimeScope, SimpleTabItemInfo simpleTabItemInfo)
    {
        LifetimeScope = lifetimeScope;
        SimpleTabItemInfo = simpleTabItemInfo;
        TabReference = new TabReference(simpleTabItemInfo.Uri);
    }

    private ILifetimeScope LifetimeScope { get; }

    private SimpleTabItemInfo SimpleTabItemInfo { get; }

    public string Host => SimpleTabItemInfo.Host;

    public string Icon => SimpleTabItemInfo.Icon;

    public object View => _view ??= LifetimeScope.Resolve(SimpleTabItemInfo.ViewType);

    public Uri Uri => SimpleTabItemInfo.Uri;

    public string Scheme => SimpleTabItemInfo.Scheme;

    public string Title => SimpleTabItemInfo.Title;

    public bool Localization => SimpleTabItemInfo.Localization;

    public void Dispose()
    {
        View.TryDispose();
    }

    public TabReference TabReference { get; }

    public Guid Id => TabReference.Id;

    public async Task LoadAsync()
    {
        await Task.CompletedTask;
    }

    public bool OnClosing()
    {
        return true;
    }
}