using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.WebView2.Commands;

[Register]
internal class OpenDevToolsWindowCommand : CommandBase, IViewSetter<WebViewHostBase>
{
    public WebViewHostBase? View { get; set; }

    public bool Disposing { get; set; }

    protected override void InternalExecute(object? parameter)
    {
        View!.OpenDevToolsWindow();
    }

    public override bool CanExecute(object? parameter)
    {
        return !View!.IsCoreWebView2Null();
    }
}