using System.Diagnostics;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Mock.Abstractions;

namespace ZYC.Framework.Modules.Mock;

[RegisterSingleInstanceAs(typeof(IMockTabItemFactory))]
internal class MockTabItemFactory : IMockTabItemFactory
{
    public MockTabItemFactory(IMockMainMenuItemsProvider mockMainMenuItemsProvider)
    {
        MockMainMenuItemsProvider = mockMainMenuItemsProvider;
    }

    private IMockMainMenuItemsProvider MockMainMenuItemsProvider { get; }

    private IList<MockTabItemInfo> MockTabItemInfos { get; } = new List<MockTabItemInfo>();

    public int Priority => 0;

    public bool IsSingle => true;

    public async Task<ITabItemInstance> CreateTabItemInstanceAsync(
        TabItemCreationContext context)
    {
        await Task.CompletedTask;
        var info = MockTabItemInfos.Where(t => t.Uri == context.Uri).ToArray().FirstOrDefault();
        if (info == null)
        {
            Debugger.Break();
        }

        return context.Resolve<MockTabItem>(new TypedParameter(typeof(MockTabItemInfo), info));
    }

    public async Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        await Task.CompletedTask;
        var result = MockTabItemInfos.Any(t => t.Uri == uri);

        return result;
    }

    public void RegisterMockTabItem(MockTabItemInfo info)
    {
        MockMainMenuItemsProvider.RegisterSubItem(info);
        MockTabItemInfos.Add(info);
    }
}