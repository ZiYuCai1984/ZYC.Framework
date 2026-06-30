using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.WindowTitle;
using ZYC.Framework.Modules.Accounts.UI;

namespace ZYC.Framework.Modules.Accounts;

[RegisterSingleInstance]
internal class AccountsWindowTitleItem : IWindowTitleExtendItem
{
    public AccountsWindowTitleItem(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    public object View => LifetimeScope.Resolve<AccountsWindowTitleItemView>();
}