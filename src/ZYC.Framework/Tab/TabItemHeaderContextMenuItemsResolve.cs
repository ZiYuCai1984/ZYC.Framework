using System.Windows.Markup;
using Autofac;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Resources;

namespace ZYC.Framework.Tab;

internal class TabItemHeaderContextMenuItemsResolve : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var lifetimeScope = LifetimeScopeResource.GetRootLifetimeScopeFromMainWindowDataContext();

        var items = lifetimeScope.Resolve<ITabItemHeaderContextMenuItemView[]>();

        return items.OrderBy(t => t.Order);
    }
}