using System.IO;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.WebBrowser.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser;

[RegisterSingleInstanceAs(typeof(IWebBrowserUriPolicy))]
internal class WebBrowserUriPolicy : IWebBrowserUriPolicy
{
    private static readonly HashSet<string> AllowedSchemes = new(StringComparer.OrdinalIgnoreCase)
    {
        Uri.UriSchemeHttp,
        Uri.UriSchemeHttps,
        Uri.UriSchemeFile,
        "chrome-extension",
        "extension",
        "chrome",
        "edge",
        "about",
        "data",
        "blob",
        "javascript"
    };

    private HashSet<string> ExcludedFileExt { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe"
    };

    public void AddExcludedFileExt(params string[] ext)
    {
        foreach (var e in ext)
        {
            ExcludedFileExt.Add(e);
        }
    }

    public bool IsAllowed(Uri uri)
    {
        if (uri is null)
        {
            return false;
        }

        if (!AllowedSchemes.Contains(uri.Scheme))
        {
            return false;
        }

        if (uri.IsFile)
        {
            var localPath = uri.LocalPath;
            if (string.IsNullOrWhiteSpace(localPath))
            {
                return false;
            }


            var ext = Path.GetExtension(localPath);
            if (string.IsNullOrEmpty(ext))
            {
                return true;
            }

            return !ExcludedFileExt.Contains(ext);
        }

        if (uri.Scheme is "http" or "https")
        {
            if (string.IsNullOrWhiteSpace(uri.Host))
            {
                return false;
            }
        }

        return true;
    }
}