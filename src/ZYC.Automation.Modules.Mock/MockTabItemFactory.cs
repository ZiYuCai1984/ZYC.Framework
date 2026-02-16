using System.Diagnostics;
using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Mock.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Mock;

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