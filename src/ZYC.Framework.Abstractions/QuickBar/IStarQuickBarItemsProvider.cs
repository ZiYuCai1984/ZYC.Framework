namespace ZYC.Framework.Abstractions.QuickBar;

/// <summary>
///     A specialized provider that manages "Starred" or "Favorite" items within the QuickBar.
///     It links unique URI identifiers to QuickBar items for persistence and tracking.
/// </summary>
public interface IStarQuickBarItemsProvider : IQuickBarItemsProvider
{
    /// <summary>
    ///     Checks if a specific resource, identified by its <see cref="Uri" />, is already starred.
    /// </summary>
    /// <param name="uri">The unique identifier of the resource.</param>
    /// <returns>True if the item is starred; otherwise, false.</returns>
    bool CheckIsStared(Uri uri);

    /// <summary>
    ///     Removes a starred item from the QuickBar based on its unique <see cref="Uri" />.
    /// </summary>
    /// <param name="uri">The unique identifier of the item to remove.</param>
    void DetachMenuItem(Uri uri);

    /// <summary>
    ///     Creates and returns a new <see cref="StarQuickBarItem" /> for the specified resource.
    /// </summary>
    /// <param name="uri">The unique identifier for the new item.</param>
    /// <param name="icon">The icon to represent the starred item.</param>
    /// <returns>A configured <see cref="StarQuickBarItem" /> instance.</returns>
    StarQuickBarItem CreateQuickMenuItem(Uri uri, string icon);
}