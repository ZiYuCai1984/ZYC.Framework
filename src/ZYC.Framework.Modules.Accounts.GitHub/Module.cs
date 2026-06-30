using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Accounts.GitHub.Abstractions;

namespace ZYC.Framework.Modules.Accounts.GitHub;

internal class Module : ModuleBase
{
    public override string Icon => GitHubAccountModuleConstants.Icon;


    public override async Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        await Task.CompletedTask;

        lifetimeScope.RegisterTabItemFactory<GitHubAuthenticationBrowserTabItemFactory>();
    }
}