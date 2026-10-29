using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

/// <summary>
///     Defines tab interactions for the file explorer module.
/// </summary>
public interface IFileExplorerTabItemInstance : ITabItemInstance
{
    /// <summary>
    ///     Updates the tab reference and notifies the manager after the displayed directory changes.
    /// </summary>
    /// <param name="newUri">The explorer's actual current URI.</param>
    /// <returns>A task representing state synchronization; it does not request navigation.</returns>
    Task NotifyUriChangedAsync(Uri newUri);

    /// <summary>
    ///     Updates the tab icon.
    /// </summary>
    /// <param name="base64Icon">The base64-encoded icon content.</param>
    void UpdateIcon(string base64Icon);
}
