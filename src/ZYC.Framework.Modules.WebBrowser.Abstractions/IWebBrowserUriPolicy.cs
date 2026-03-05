namespace ZYC.Framework.Modules.WebBrowser.Abstractions;

/// <summary>
///     Defines a policy that determines whether a URI can be opened in the web browser.
/// </summary>
public interface IWebBrowserUriPolicy
{
    /// <summary>
    ///     Determines whether the specified URI is allowed to be opened.
    /// </summary>
    /// <param name="uri">The URI to evaluate.</param>
    /// <returns>
    ///     <c>true</c> if the URI is allowed; otherwise, <c>false</c>.
    /// </returns>
    bool IsAllowed(Uri uri);

    /// <summary>
    ///     Adds file extensions that should be excluded from navigation.
    /// </summary>
    /// <param name="ext">
    ///     The file extensions to exclude (for example: <c>.xml</c>, <c>.json</c>).
    /// </param>
    void AddExcludedFileExt(params string[] ext);
}