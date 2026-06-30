namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Contains non-secret profile data for a signed-in account.
/// </summary>
public class AccountProfile
{
    /// <summary>
    ///     Gets or sets the provider id.
    /// </summary>
    public string ProviderId { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the provider-specific user id.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the user name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    ///     Gets or sets the email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     Gets or sets the avatar URI.
    /// </summary>
    public Uri? AvatarUri { get; set; }
}
