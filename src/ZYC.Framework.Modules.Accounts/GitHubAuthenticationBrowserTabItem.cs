using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Accounts.UI;

namespace ZYC.Framework.Modules.Accounts;

[Register]
internal class GitHubAuthenticationBrowserTabItem : TabItemInstanceBase<GitHubAuthenticationBrowserView>
{
    public GitHubAuthenticationBrowserTabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference,
        Uri authorizationUri) : base(lifetimeScope, tabReference)
    {
        AuthorizationUri = authorizationUri;
    }

    private Uri AuthorizationUri { get; }

    public override string Title => "GitHub sign-in";

    public override string Icon => "Github";

    public override object View => _view ??= LifetimeScope.Resolve<GitHubAuthenticationBrowserView>(
        new TypedParameter(typeof(Uri), AuthorizationUri));
}
