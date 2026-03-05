using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.WebView2;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.WebView2;

public abstract class CycleWebViewHostBase : WebViewHostBase
{
    protected CycleWebViewHostBase(ILifetimeScope lifetimeScope, ILogger<CycleWebViewHostBase> logger) : base(
        lifetimeScope)
    {
        Logger = logger;
    }

    private ILogger<CycleWebViewHostBase> Logger { get; }

    public abstract string BaseUri { get; set; }

    public virtual int PostNavigationDelayMs => Random.Shared.Next(1200, 2200);

    public virtual int AutoReloadDelayMs => 9999;

    public virtual bool IsAutoReload { get; set; }

    protected override async Task InternalWebViewHostLoadedAsync()
    {
        if (IsAutoReload)
        {
            var script = CoreWebView2Tools.GetAutoReloadScript(BaseUri, AutoReloadDelayMs);
            await CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script);
        }

        await NavigateAsync(BaseUri);
    }

    protected override async void InternalOnNavigationCompleted(object? sender,
        CoreWebView2NavigationCompletedEventArgs args)
    {
        try
        {
            await Task.Delay(PostNavigationDelayMs);

            if (CoreWebView2.Source != BaseUri)
            {
                await NavigateAsync(BaseUri);
                return;
            }

            await OnBaseUriReachedAsync();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }


    protected virtual Task OnBaseUriReachedAsync()
    {
        return Task.CompletedTask;
    }
}