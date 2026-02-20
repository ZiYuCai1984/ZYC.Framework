using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

/// <summary>
///     Defines tab interactions for the file explorer module.
/// </summary>
public interface IFileExplorerTabItemInstance : ITabItemInstance
{
    /// <summary>
    ///     Notifies the tab that an internal navigation is about to occur.
    /// </summary>
    /// <param name="newUri">The destination URI.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task TabInternalNavigatingAsync(Uri newUri);

    /// <summary>
    ///     Updates the tab icon.
    /// </summary>
    /// <param name="base64Icon">The base64-encoded icon content.</param>
    void UpdateIcon(string base64Icon);
}