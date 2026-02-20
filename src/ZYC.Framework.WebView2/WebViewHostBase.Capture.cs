using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit;

namespace ZYC.Framework.WebView2;

public partial class WebViewHostBase
{
    public CoreWebView2CapturePreviewImageFormat CoreWebView2CapturePreviewImageFormat { get; private set; } =
        CoreWebView2CapturePreviewImageFormat.Png;

    public async Task<Stream> CaptureAsync()
    {
        var s = new MemoryStream();
        await CoreWebView2.CapturePreviewAsync(
            CoreWebView2CapturePreviewImageFormat,
            s);
        return s;
    }


    private static string GetBoundingClientRectScript(string getElementScript)
    {
        var s = "function getElementViewportRect(element)";
        s += "{";
        s += "rect=element.getBoundingClientRect();";
        s +=
            "return {top:rect.top/window.innerHeight,left:rect.left/window.innerWidth,width:rect.width/window.innerWidth,height:rect.height/window.innerHeight};";
        s += "}";
        s += $"getElementViewportRect({getElementScript});";

        return s;
    }


    public async Task<byte[]> CaptureElementAsync(string getElementScript)
    {
        var script = GetBoundingClientRectScript(getElementScript);

        var result = await ExecuteScriptAsync(script);

        var domRect = JsonTools.Deserialize<PercentDomRect>(result)!;

        await using var imageStream = await CaptureAsync();
        using var originalImage = new Bitmap(imageStream);

        using var croppedImage = originalImage.Clone(
            GetRectangleF(originalImage, domRect),
            originalImage.PixelFormat);

        using var outputMs = new MemoryStream();
        croppedImage.Save(outputMs, ImageFormat.Png);
        return outputMs.ToArray();
    }

    public Task<byte[]> CaptureElementByClassNameAsync(string className, int index = 0)
    {
        return CaptureElementAsync($"{Document_GetElementsByClassName}('{className}')[{index}]");
    }

    public Task<byte[]> CaptureElementByIdAsync(string id)
    {
        return CaptureElementAsync($"{Document_GetElementById}('{id}')");
    }

    public Task<byte[]> CaptureElementByNameAsync(string name, int index = 0)
    {
        return CaptureElementAsync($"{Document_GetElementsByName}('{name}')[{index}]");
    }

    public Task<byte[]> CaptureElementByTagNameAsync(string tagName, int index = 0)
    {
        return CaptureElementAsync($"{Document_GetElementsByTagName}('{tagName}')[{index}]");
    }

    private static RectangleF GetRectangleF(Bitmap bitmap, PercentDomRect percentDomRect)
    {
        var left = (float)percentDomRect.left;
        var top = (float)percentDomRect.top;

        var width = (float)percentDomRect.width;
        var height = (float)percentDomRect.height;

        if (left < 0)
        {
            left = 0;
        }

        if (top < 0)
        {
            top = 0;
        }

        if (width > 1 - left)
        {
            width = 1 - left;
        }

        if (height > 1 - top)
        {
            height = 1 - top;
        }


        var w = bitmap.Width;
        var h = bitmap.Height;

        return new RectangleF(left * w, top * h, width * w, height * h);
    }

    public void SetCoreWebView2CapturePreviewImageFormat(CoreWebView2CapturePreviewImageFormat format)
    {
        CoreWebView2CapturePreviewImageFormat = format;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class PercentDomRect
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable InconsistentNaming
        public double width { get; set; }
        public double height { get; set; }
        public double top { get; set; }
        public double left { get; set; }
    }
}