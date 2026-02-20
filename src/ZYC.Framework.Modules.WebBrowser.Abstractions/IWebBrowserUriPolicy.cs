namespace ZYC.Framework.Modules.WebBrowser.Abstractions;

public interface IWebBrowserUriPolicy
{
    bool IsAllowed(Uri uri);

    /// <summary>
    ///     ext: .xml, .json
    /// </summary>
    /// <param name="ext"></param>
    void AddExcludedFileExt(params string[] ext);
}