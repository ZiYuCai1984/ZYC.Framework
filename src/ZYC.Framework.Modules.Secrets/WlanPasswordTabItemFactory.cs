using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.Secrets;

[RegisterSingleInstance]
internal class WlanPasswordTabItemFactory : ITabItemFactory
{
    public bool IsSingle => true;

    public int Priority => 0;

    public async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;

        return context.Resolve<WlanPasswordTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }

    public async Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        await Task.CompletedTask;
        if (uri.Host == WlanPasswordTabItem.Constants.Host)
        {
            return true;
        }

        return false;
    }
}