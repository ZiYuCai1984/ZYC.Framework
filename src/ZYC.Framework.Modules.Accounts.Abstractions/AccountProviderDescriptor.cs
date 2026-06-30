namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Describes an account provider that can authenticate a user.
/// </summary>
public class AccountProviderDescriptor
{
    /// <summary>
    ///     Gets or sets the stable provider id.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the provider display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the provider icon.
    /// </summary>
    public string Icon { get; set; } = "AccountCircleOutline";

    /// <summary>
    ///     Gets or sets a value indicating whether the provider can be used.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}
