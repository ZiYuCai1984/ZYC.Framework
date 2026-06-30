namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Authenticates users through a specific identity provider.
/// </summary>
public interface IAccountProvider
{
    /// <summary>
    ///     Gets provider metadata.
    /// </summary>
    AccountProviderDescriptor Descriptor { get; }

    /// <summary>
    ///     Gets a cached non-secret session if one is available.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The cached session, or null.</returns>
    Task<AccountSession?> GetCachedSessionAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Starts provider sign-in.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The resulting account session.</returns>
    Task<AccountSession> SignInAsync(AccountLoginRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Acquires an access token for the provider.
    /// </summary>
    /// <param name="request">The token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    Task<string> AcquireTokenAsync(AccountTokenRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Signs out of the provider and removes local provider tokens.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SignOutAsync(CancellationToken cancellationToken);
}
