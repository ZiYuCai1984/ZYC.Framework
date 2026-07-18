using Autofac;
using Microsoft.AspNetCore.WebUtilities;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.Accounts.GitHub;

[RegisterSingleInstance]
[TabItemRoute(Host = GitHubAuthenticationBrowserRoute.Host, Path = GitHubAuthenticationBrowserRoute.SignInPath)]
internal class GitHubAuthenticationBrowserTabItemFactory : TabItemFactoryBase
{
    private const string AuthorizationUriParameter = "authorizationUri";

    public override bool IsSingle => false;

    public static Uri CreateUri(Uri authorizationUri)
    {
        return UriTools.CreateAppUri(
            GitHubAuthenticationBrowserRoute.Host,
            GitHubAuthenticationBrowserRoute.SignInPath,
            $"{AuthorizationUriParameter}={Uri.EscapeDataString(authorizationUri.ToString())}");
    }

    public override Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        if (!string.Equals(uri.Scheme, ProductInfo.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(false);
        }

        return base.CheckUriMatchedAsync(uri);
    }

    public override Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        var query = QueryHelpers.ParseQuery(context.Uri.Query);
        var rawAuthorizationUri = query.GetValueOrDefault(AuthorizationUriParameter).ToString();
        if (!Uri.TryCreate(rawAuthorizationUri, UriKind.Absolute, out var authorizationUri))
        {
            throw new InvalidOperationException("GitHub authorization URI is missing.");
        }

        return Task.FromResult<ITabItemInstance>(
            context.Resolve<GitHubAuthenticationBrowserTabItem>(
                new TypedParameter(typeof(TabReference), new TabReference(context.Uri)),
                new TypedParameter(typeof(Uri), authorizationUri)));
    }
}

internal static class GitHubAuthenticationBrowserRoute
{
    public const string Host = "github-authentication";

    public const string SignInPath = "/sign-in";
}
