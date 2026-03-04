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
    ///     Called internally when the tab is about to navigate to a new URI.
    /// </summary>
    /// <param name="newUri">The URI that will be navigated to.</param>
    /// <returns>A task representing the asynchronous navigation operation.</returns>
    Task TabInternalNavigatingAsync(Uri newUri);
}