namespace ZYC.Framework.Modules.Accounts.Abstractions;

/// <summary>
///     Stores provider token payloads outside normal config and state files.
/// </summary>
public interface IAccountTokenStore
{
    /// <summary>
    ///     Gets a protected provider token payload.
    /// </summary>
    /// <param name="providerId">The provider id.</param>
    /// <param name="key">The provider-specific key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The unprotected payload, or null.</returns>
    Task<byte[]?> GetAsync(string providerId, string key, CancellationToken cancellationToken);

    /// <summary>
    ///     Stores a provider token payload.
    /// </summary>
    /// <param name="providerId">The provider id.</param>
    /// <param name="key">The provider-specific key.</param>
    /// <param name="payload">The unprotected payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync(string providerId, string key, byte[] payload, CancellationToken cancellationToken);

    /// <summary>
    ///     Removes a provider token payload.
    /// </summary>
    /// <param name="providerId">The provider id.</param>
    /// <param name="key">The provider-specific key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string providerId, string key, CancellationToken cancellationToken);

    /// <summary>
    ///     Removes all token payloads owned by a provider.
    /// </summary>
    /// <param name="providerId">The provider id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ClearProviderAsync(string providerId, CancellationToken cancellationToken);
}
