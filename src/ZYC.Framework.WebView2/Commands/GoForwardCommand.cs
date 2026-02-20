using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.WebView2.Commands;

[Register]
internal class GoForwardCommand : CommandBase, IViewSetter<WebViewHostBase>
{
    public WebViewHostBase? View { get; set; }

    public bool Disposing { get; set; }

    public override bool CanExecute(object? parameter)
    {
        if (View == null)
        {
            return false;
        }

        return View.CanGoForward();
    }

    protected override void InternalExecute(object? parameter)
    {
        View?.GoForward();
    }
}