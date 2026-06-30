namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Coordinates account providers and account session state.
/// </summary>
public interface IAccountManager
{
    /// <summary>
    ///     Initializes cached account sessions.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InitializeAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Gets all registered provider descriptors.
    /// </summary>
    /// <returns>The provider descriptors.</returns>
    AccountProviderDescriptor[] GetProviders();

    /// <summary>
    ///     Gets the current active account session.
    /// </summary>
    /// <returns>The current account session, or null.</returns>
    AccountSession? GetCurrentSession();

    /// <summary>
    ///     Gets a session by provider id.
    /// </summary>
    /// <param name="providerId">The provider id.</param>
    /// <returns>The session, or null.</returns>
    AccountSession? GetSession(string providerId);

    /// <summary>
    ///     Gets a session by provider id.
    /// </summary>
    /// <param name="providerId">The provider id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The session, or null.</returns>
    Task<AccountSession?> GetSessionAsync(string providerId, CancellationToken cancellationToken);

    /// <summary>
    ///     Signs in with a provider.
    /// </summary>
    /// <param name="providerId">The provider id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The resulting account session.</returns>
    Task<AccountSession> SignInAsync(string providerId, CancellationToken cancellationToken);

    /// <summary>
    ///     Signs out of a provider.
    /// </summary>
    /// <param name="providerId">The provider id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SignOutAsync(string providerId, CancellationToken cancellationToken);

    /// <summary>
    ///     Acquires an access token from a provider.
    /// </summary>
    /// <param name="providerId">The provider id.</param>
    /// <param name="scopes">The requested scopes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    Task<string> AcquireTokenAsync(string providerId, string[] scopes, CancellationToken cancellationToken);
}
