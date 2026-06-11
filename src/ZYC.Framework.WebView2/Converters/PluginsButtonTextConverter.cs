using Microsoft.Web.WebView2.Core;
using ZYC.Framework.Core.Converters;
using ZYC.Framework.Core.Localizations;

namespace ZYC.Framework.WebView2.Converters;

internal class PluginsButtonTextConverter : ValueConverterBase<CoreWebView2BrowserExtension[], string>
{
    protected override string InternalConvert(CoreWebView2BrowserExtension[] value)
    {
        return $"{L.Translate("Plugins")} ({value.Length})";
    }

    protected override CoreWebView2BrowserExtension[] InternalConvertBack(string value)
    {
        throw new NotSupportedException();
    }
}