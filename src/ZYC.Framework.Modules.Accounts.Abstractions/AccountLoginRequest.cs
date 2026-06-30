namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Describes a provider login request.
/// </summary>
public class AccountLoginRequest
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
    ///     Gets or sets a value indicating whether the provider should force an interactive account picker.
    /// </summary>
    public bool ForceLogin { get; set; }
}
