namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Describes an access token acquisition request.
/// </summary>
public class AccountTokenRequest
{
    /// <summary>
    ///     Gets or sets the provider id.
    /// </summary>
    public string ProviderId { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets requested scopes.
    /// </summary>
    public string[] Scopes { get; set; } = [];

    /// <summary>
    ///     Gets or sets a value indicating whether silent acquisition should be skipped.
    /// </summary>
    public bool ForceRefresh { get; set; }
}
