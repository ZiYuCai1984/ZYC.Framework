using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.StatusBar;
using ZYC.Framework.Modules.Accounts.UI;

namespace ZYC.Framework.Modules.Accounts;

[RegisterSingleInstance]
internal class AccountsStatusBarItem : IStatusBarItem
{
    public AccountsStatusBarItem(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    public object View => LifetimeScope.Resolve<AccountsStatusBarItemView>();

    public StatusBarSection Section => StatusBarSection.Left;

    public int Order => -1000;
}
