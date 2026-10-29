namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Optional capability for navigating within an existing tab instance.
/// </summary>
public interface INavigableTabItemInstance : ITabItemInstance
{
    /// <summary>
    ///     Submits navigation to this tab without creating another tab or changing focus.
    ///     After its content changes URI, update the tab reference and call
    ///     <see cref="ITabManager.NotifyUriChangedAsync" />.
    /// </summary>
    /// <param name="uri">The requested absolute URI.</param>
    /// <returns>
    ///     True when the request is accepted, without waiting for content to finish loading;
    ///     false only when this tab cannot handle the URI, with no navigation side effects.
    ///     Failures and cancellation must throw rather than return false.
    /// </returns>
    Task<bool> TryNavigateAsync(Uri uri);
}
