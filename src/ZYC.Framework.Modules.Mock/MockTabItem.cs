using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Modules.Mock.Abstractions;

namespace ZYC.Framework.Modules.Mock;

[Register]
internal class MockTabItem : ITabItemInstance
{
    public MockTabItem(ILifetimeScope lifetimeScope, MockTabItemInfo mockTabItemInfo)
    {
        LifetimeScope = lifetimeScope;
        MockTabItemInfo = mockTabItemInfo;
        TabReference = new TabReference(mockTabItemInfo.Uri);
    }

    private ILifetimeScope LifetimeScope { get; }

    private MockTabItemInfo MockTabItemInfo { get; }

    public string Scheme => ProductInfo.Scheme;

    public TabReference TabReference { get; }

    public Guid Id => TabReference.Id;

    public Uri Uri => MockTabItemInfo.Uri;

    public string Host => Uri.Host;

    public string Icon => MockTabItemInfo.Icon;

    public string Title => MockTabItemInfo.Title;

    public object View => LifetimeScope.Resolve(MockTabItemInfo.ViewType);

    public bool Localization => false;

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public bool OnClosing()
    {
        return true;
    }

    public void Dispose()
    {
        View.TryDispose();
    }

    public async ValueTask DisposeAsync()
    {
        await LifetimeScope.DisposeAsync();
    }
}