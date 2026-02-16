using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Secrets;

[RegisterSingleInstance]
internal class PasswordGeneratorTabItemFactory : ITabItemFactory
{
    public bool IsSingle => true;

    public int Priority => 0;

    public async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;

        return context.Resolve<PasswordGeneratorTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }

    public async Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        await Task.CompletedTask;
        if (uri.Host == PasswordGeneratorTabItem.Constants.Host)
        {
            return true;
        }

        return false;
    }
}