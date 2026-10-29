using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Modules.WebBrowser.Abstractions;

/// <summary>
///     Represents a tab instance capable of displaying web content.
/// </summary>
public interface IWebTabItemInstance : ITabItemInstance
{
    /// <summary>
    ///     Sets the title of the tab.
    /// </summary>
    /// <param name="title">The new tab title.</param>
    void SetTitle(string title);

    /// <summary>
    ///     Sets the icon associated with the tab.
    /// </summary>
    /// <param name="icon">The icon identifier or resource.</param>
    void SetIcon(string icon);

    /// <summary>
    ///     Updates the tab reference and notifies the manager after the browser's URI changes.
    /// </summary>
    /// <param name="newUri">The browser's actual current URI.</param>
    /// <returns>A task representing state synchronization; it does not request navigation.</returns>
    Task NotifyUriChangedAsync(Uri newUri);
}
