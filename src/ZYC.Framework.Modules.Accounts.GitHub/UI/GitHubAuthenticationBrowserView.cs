using Autofac;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
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

        var query = ParseQuery(uri.Query);
        CallbackBroker.TryComplete(
            new GitHubAuthenticationCallback
            {
                Code = query.GetValueOrDefault("code") ?? "",
                State = query.GetValueOrDefault("state") ?? "",
                Nonce = GetNonce(query),
                Error = query.GetValueOrDefault("error") ?? "",
                ErrorDescription = query.GetValueOrDefault("error_description") ?? ""
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
               && string.Equals(NormalizePath(uri.AbsolutePath), NormalizePath(configured.AbsolutePath),
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

    private static string GetNonce(Dictionary<string, string> query)
    {
        if (query.TryGetValue("nonce", out var nonce)
            && !string.IsNullOrWhiteSpace(nonce))
        {
            return nonce;
        }

        if (!query.TryGetValue("state", out var state)
            || string.IsNullOrWhiteSpace(state))
        {
            return "";
        }

        var decodedState = DecodeRepeatedly(state);
        if (!Uri.TryCreate(decodedState, UriKind.Absolute, out var stateUri))
        {
            return "";
        }

        var stateQuery = ParseQuery(stateUri.Query);
        return stateQuery.GetValueOrDefault("nonce") ?? "";
    }

    private static Dictionary<string, string> ParseQuery(string query)
    {
        if (query.StartsWith("?", StringComparison.Ordinal))
        {
            query = query[1..];
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var equalsIndex = pair.IndexOf('=', StringComparison.Ordinal);
            var key = equalsIndex < 0 ? pair : pair[..equalsIndex];
            var value = equalsIndex < 0 ? "" : pair[(equalsIndex + 1)..];
            values[Uri.UnescapeDataString(key.Replace("+", " ", StringComparison.Ordinal))] =
                Uri.UnescapeDataString(value.Replace("+", " ", StringComparison.Ordinal));
        }

        return values;
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

    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "/";
        }

        path = path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
        return path.Length > 1 ? path.TrimEnd('/') : path;
    }
}
