using Autofac;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.WebView2.Dialog;

namespace ZYC.Framework.WebView2.Commands;

[Register]
public class ShowPluginsDialogViewCommand : CommandBase<CoreWebView2BrowserExtension[]>
{
    public ShowPluginsDialogViewCommand(
        ILifetimeScope lifetimeScope,
        IDialogManager dialogManager)
    {
        LifetimeScope = lifetimeScope;
        DialogManager = dialogManager;
    }

    private ILifetimeScope LifetimeScope { get; }

    private IDialogManager DialogManager { get; }

    protected override void InternalExecute(CoreWebView2BrowserExtension[] coreWebView2BrowserExtensions)
    {
        DialogManager.Show<PluginsDialogView>(
            new TypedParameter(typeof(CoreWebView2BrowserExtension[]), coreWebView2BrowserExtensions));
    }
}