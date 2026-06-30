using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Configures built-in account providers.
/// </summary>
public class AccountsConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the client id of the application's GitHub OAuth app.
    /// </summary>
    public string GitHubClientId { get; set; } = "01ab8ac9400c4e429b23";

    /// <summary>
    ///     Gets or sets the client secret used when exchanging authorization codes directly with GitHub.
    /// </summary>
    public string GitHubClientSecret { get; set; } = "";

    /// <summary>
    ///     Gets or sets the redirect URI sent to GitHub for the browser relay callback.
    /// </summary>
    public string GitHubRedirectUri { get; set; } = "https://vscode.dev/redirect";

    /// <summary>
    ///     Gets or sets the deep-link URI embedded in the OAuth state when using a redirect relay.
    /// </summary>
    public string GitHubDeepLinkUri { get; set; } =
        "vscode://vscode.github-authentication/did-authenticate";

    /// <summary>
    ///     Gets or sets the login callback timeout in seconds.
    /// </summary>
    public int GitHubAuthorizationTimeoutSeconds { get; set; } = 4096;

    /// <summary>
    ///     Gets or sets an optional server-side token exchange endpoint owned by the application.
    /// </summary>
    public string GitHubTokenExchangeEndpoint { get; set; } =
        "https://vscode.dev/codeExchangeProxyEndpoints/github/login/oauth/access_token";

    /// <summary>
    ///     Gets or sets the GitHub OAuth scopes requested by default.
    /// </summary>
    public string[] GitHubScopes { get; set; } = ["read:user", "user:email"];

    /// <summary>
    ///     Gets or sets the GitHub REST API version header value.
    /// </summary>
    public string GitHubApiVersion { get; set; } = "2026-03-10";
}