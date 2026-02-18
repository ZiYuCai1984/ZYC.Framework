namespace ZYC.Automation.Modules.Update.Abstractions;

/// <summary>
///     Provides high-level operations for managing product updates, including fetching update metadata,
///     downloading the update payload, and applying the update.
/// </summary>
/// <remarks>
///     Implementations are expected to maintain and update an <see cref="UpdateContext" /> that reflects
///     the latest known update state. Methods returning <see cref="UpdateContext" /> should return the
///     updated context after the operation completes.
/// </remarks>
public interface IUpdateManager
{
    /// <summary>
    ///     Gets the current update context representing the latest known update state.
    /// </summary>
    /// <returns>
    ///     The current <see cref="UpdateContext" /> instance.
    /// </returns>
    UpdateContext GetCurrentUpdateContext();

    /// <summary>
    ///     Fetches metadata for a new product update (if available) and updates the current context.
    /// </summary>
    /// <param name="token">
    ///     A cancellation token used to cancel the operation.
    /// </param>
    /// <returns>
    ///     A task that completes with the updated <see cref="UpdateContext" />.
    /// </returns>
    Task<UpdateContext> FetchNewProductInfoAsync(CancellationToken token);

    /// <summary>
    ///     Downloads the update payload for the specified product and updates the current context with download progress and
    ///     results.
    /// </summary>
    /// <param name="product">
    ///     The target product update to download.
    /// </param>
    /// <param name="token">
    ///     A cancellation token used to cancel the operation.
    /// </param>
    /// <returns>
    ///     A task that completes with the updated <see cref="UpdateContext" />.
    /// </returns>
    Task<UpdateContext> DownloadProductAsync(NewProduct product, CancellationToken token);


    /// <summary>
    ///     Downloads the update payload for the specified product and updates the current context with download progress and
    ///     results.
    /// </summary>
    /// <param name="product">
    ///     The target product update to download.
    /// </param>
    /// <returns>
    ///     A task that completes with the updated <see cref="UpdateContext" />.
    /// </returns>
    Task<UpdateContext> DownloadProductAsync(NewProduct product)
    {
        return DownloadProductAsync(product, CancellationToken.None);
    }

    /// <summary>
    ///     Applies the specified product update (e.g., install/patch/replace) and updates the current context accordingly.
    /// </summary>
    /// <param name="product">
    ///     The target product update to apply.
    /// </param>
    /// <returns>
    ///     A task that completes with the updated <see cref="UpdateContext" />.
    /// </returns>
    Task<UpdateContext> ApplyProductAsync(NewProduct product);
}