using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Modules.ChromeExtensions.UI;

[Register]
internal partial class ChromeWebStoreBrowserDialogWindow
{
    public ChromeWebStoreBrowserDialogWindow(ChromeWebStoreBrowserView storeBrowserView)
    {
        StoreBrowserView = storeBrowserView;
        InitializeComponent();
    }

    public ChromeWebStoreBrowserView StoreBrowserView { get; }

    protected override void OnClosed(EventArgs e)
    {
        StoreBrowserView.Dispose();
        base.OnClosed(e);
    }
}