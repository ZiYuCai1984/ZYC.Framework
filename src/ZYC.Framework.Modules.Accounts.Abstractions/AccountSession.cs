namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Represents a non-secret account session snapshot.
/// </summary>
public class AccountSession
{
    /// <summary>
    ///     Gets or sets the account profile.
    /// </summary>
    public AccountProfile Profile { get; set; } = new();

    /// <summary>
    ///     Gets or sets granted or requested scopes.
    /// </summary>
    public string[] Scopes { get; set; } = [];

    /// <summary>
    ///     Gets or sets the known token expiration time.
    /// </summary>
    public DateTimeOffset? ExpiresOn { get; set; }
}
