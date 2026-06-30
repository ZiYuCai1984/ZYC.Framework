using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.WindowTitle;
using ZYC.Framework.Modules.Accounts.Abstractions;

namespace ZYC.Framework.Modules.Accounts;

internal class Module : ModuleBase
{
    public override string Icon => AccountsModuleConstants.Icon;

    public override async Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        var accountManager = lifetimeScope.Resolve<IAccountManager>();
        await accountManager.InitializeAsync(CancellationToken.None);

        var windowTitleExtendManager = lifetimeScope.Resolve<IWindowTitleExtendManager>();
        windowTitleExtendManager.RegisterItem<AccountsWindowTitleItem>();
    }
}