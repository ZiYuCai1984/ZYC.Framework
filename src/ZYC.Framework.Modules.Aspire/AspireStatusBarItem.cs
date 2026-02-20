using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.StatusBar;
using ZYC.Framework.Modules.Aspire.UI;

namespace ZYC.Framework.Modules.Aspire;

[RegisterSingleInstance]
internal class AspireStatusBarItem : IStatusBarItem
{
    public AspireStatusBarItem(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    public object View => LifetimeScope.Resolve<AspireStatusBarItemView>();

    public StatusBarSection Section => StatusBarSection.Right;
}