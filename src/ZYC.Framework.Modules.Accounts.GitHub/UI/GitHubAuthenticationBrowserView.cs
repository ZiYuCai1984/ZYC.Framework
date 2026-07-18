using Autofac;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.Accounts.GitHub.Abstractions;
using ZYC.Framework.WebView2;

namespace ZYC.Framework.Modules.Accounts.GitHub.UI;

[Register]
internal class GitHubAuthenticationBrowserView : WebViewHostBase
{
    public GitHubAuthenticationBrowserView(
        ILifetimeScope lifetimeScope,
        GitHubAccountConfig accountsConfig,
        GitHubAuthenticationCallbackBroker callbackBroker,
        Uri authorizationUri) : base(lifetimeScope)
    {
        AccountsConfig = accountsConfig;
        CallbackBroker = callbackBroker;
        AuthorizationUri = authorizationUri;
    }

    private GitHubAccountConfig AccountsConfig { get; }

    private GitHubAuthenticationCallbackBroker CallbackBroker { get; }

    private Uri AuthorizationUri { get; }

    protected override async Task InternalWebViewHostLoadedAsync()
    {
        await NavigateAsync(AuthorizationUri);
    }

    protected override void OnLaunchingExternalUriScheme(
        object? sender,
        CoreWebView2LaunchingExternalUriSchemeEventArgs e)
    {
        if (TryHandleCallbackUri(e.Uri))
        {
            e.Cancel = true;
            return;
        }

        base.OnLaunchingExternalUriScheme(sender, e);
    }

    protected override void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (TryHandleCallbackUri(e.Uri))
        {
            e.Cancel = true;
            return;
        }

        base.OnNavigationStarting(sender, e);
    }

    protected override void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        if (TryHandleCallbackUri(e.Uri))
        {
            e.Handled = true;
            return;
        }

        base.OnNewWindowRequested(sender, e);
    }

    private bool TryHandleCallbackUri(string rawUri)
    {
        if (!Uri.TryCreate(rawUri, UriKind.Absolute, out var uri)
            || !IsConfiguredDeepLink(uri))
        {
            return false;
        }

        var query = QueryHelpers.ParseQuery(uri.Query);
        CallbackBroker.TryComplete(
            new GitHubAuthenticationCallback
            {
                Code = query.GetValueOrDefault("code").ToString(),
                State = query.GetValueOrDefault("state").ToString(),
                Nonce = GetNonce(query),
                Error = query.GetValueOrDefault("error").ToString(),
                ErrorDescription = query.GetValueOrDefault("error_description").ToString()
            });

        CoreWebView2.NavigateToString(
            "<!doctype html><html><head><meta charset=\"utf-8\"><title>GitHub sign-in complete</title></head>"
            + "<body><h1>GitHub sign-in complete</h1><p>You can close this tab.</p></body></html>");

        return true;
    }

    private bool IsConfiguredDeepLink(Uri uri)
    {
        var configured = GetConfiguredDeepLinkUri();
        return string.Equals(uri.Scheme, configured.Scheme, StringComparison.OrdinalIgnoreCase)
               && string.Equals(uri.Host, configured.Host, StringComparison.OrdinalIgnoreCase)
               && string.Equals(
                   UriTools.NormalizeUriPath(uri.AbsolutePath),
                   UriTools.NormalizeUriPath(configured.AbsolutePath),
                   StringComparison.OrdinalIgnoreCase);
    }

    private Uri GetConfiguredDeepLinkUri()
    {
        var rawUri = string.IsNullOrWhiteSpace(AccountsConfig.GitHubDeepLinkUri)
            ? "vscode://vscode.github-authentication/did-authenticate"
            : AccountsConfig.GitHubDeepLinkUri.Trim();

        if (!Uri.TryCreate(rawUri, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException("AccountsConfig.GitHubDeepLinkUri must be an absolute URI.");
        }

        return uri;
    }

    private static string GetNonce(Dictionary<string, StringValues> query)
    {
        if (query.TryGetValue("nonce", out var nonce)
            && !string.IsNullOrWhiteSpace(nonce.ToString()))
        {
            return nonce.ToString();
        }

        if (!query.TryGetValue("state", out var state)
            || string.IsNullOrWhiteSpace(state.ToString()))
        {
            return "";
        }

        var decodedState = DecodeRepeatedly(state.ToString());
        if (!Uri.TryCreate(decodedState, UriKind.Absolute, out var stateUri))
        {
            return "";
        }

        var stateQuery = QueryHelpers.ParseQuery(stateUri.Query);
        return stateQuery.GetValueOrDefault("nonce").ToString();
    }

    private static string DecodeRepeatedly(string value)
    {
        var current = value;
        for (var i = 0; i < 2; i++)
        {
            var decoded = Uri.UnescapeDataString(current);
            if (string.Equals(decoded, current, StringComparison.Ordinal))
            {
                break;
            }

            current = decoded;
        }

        return current;
    }
}
