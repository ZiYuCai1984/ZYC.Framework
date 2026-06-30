using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Modules.Accounts.GitHub.Abstractions;

namespace ZYC.Framework.Modules.Accounts.GitHub;

internal class GitHubAccountModule : ModuleBase
{
    public override string Icon => GitHubAccountModuleConstants.Icon;
}