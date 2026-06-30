using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Stores non-secret account session metadata.
/// </summary>
public class AccountsState : IState
{
    /// <summary>
    ///     Gets or sets the active provider id.
    /// </summary>
    public string? ActiveProviderId { get; set; }

    /// <summary>
    ///     Gets or sets cached account sessions without access or refresh tokens.
    /// </summary>
    public AccountSession[] Sessions { get; set; } = [];
}
