using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Configures built-in account providers.
/// </summary>
public class AccountsConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the Microsoft Entra public client application id.
    /// </summary>
    public string MicrosoftClientId { get; set; } = "";

    /// <summary>
    ///     Gets or sets the Microsoft tenant segment used in the authority URL.
    /// </summary>
    public string MicrosoftTenantId { get; set; } = "common";

    /// <summary>
    ///     Gets or sets the Microsoft Graph scopes requested by default.
    /// </summary>
    public string[] MicrosoftScopes { get; set; } = ["User.Read"];

    /// <summary>
    ///     Gets or sets the GitHub OAuth app client id.
    /// </summary>
    public string GitHubClientId { get; set; } = "";

    /// <summary>
    ///     Gets or sets the GitHub OAuth app client secret used for direct authorization-code exchange.
    /// </summary>
    public string GitHubClientSecret { get; set; } = "";

    /// <summary>
    ///     Gets or sets the redirect URI sent to GitHub. Leave empty to use the local loopback callback directly.
    /// </summary>
    public string GitHubRedirectUri { get; set; } = "";

    /// <summary>
    ///     Gets or sets the local loopback callback port. Use zero to bind a temporary port.
    /// </summary>
    public int GitHubLoopbackPort { get; set; }

    /// <summary>
    ///     Gets or sets the login callback timeout in seconds.
    /// </summary>
    public int GitHubAuthorizationTimeoutSeconds { get; set; } = 300;

    /// <summary>
    ///     Gets or sets an optional token exchange endpoint owned by the application.
    /// </summary>
    public string GitHubTokenExchangeEndpoint { get; set; } = "";

    /// <summary>
    ///     Gets or sets the GitHub OAuth scopes requested by default.
    /// </summary>
    public string[] GitHubScopes { get; set; } = ["read:user", "user:email"];

    /// <summary>
    ///     Gets or sets the GitHub REST API version header value.
    /// </summary>
    public string GitHubApiVersion { get; set; } = "2026-03-10";
}
