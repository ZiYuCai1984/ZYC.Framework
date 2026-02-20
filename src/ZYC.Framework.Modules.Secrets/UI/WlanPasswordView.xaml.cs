using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.BusyWindow;

namespace ZYC.Framework.Modules.Secrets.UI;

[Register]
public partial class WlanPasswordView
{
    public WlanPasswordView(IAppBusyWindow appBusyWindow)
    {
        AppBusyWindow = appBusyWindow;
    }

    private IAppBusyWindow AppBusyWindow { get; }

    protected override async void InternalOnLoaded()
    {
        //TODO Use non-blocking loading
        using (AppBusyWindow.Enqueue())
        {
            var info = await WlanTools.GetAllWlanInfoAsync();
            var markdown = MarkdownTableTools.ToMarkdownTable(info);


            MarkdownScrollViewer.Markdown = markdown;
        }
    }
}