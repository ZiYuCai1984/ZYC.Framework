using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Autofac;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Aspire.Abstractions.Event;

namespace ZYC.Framework.Modules.Aspire.UI;

[Register]
internal partial class AspireView
{
    public AspireView(
        ITabManager tabManager,
        IAspireServiceManager aspireServiceManager,
        IEventAggregator eventAggregator,
        ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        TabManager = tabManager;
        AspireServiceManager = aspireServiceManager;
        EventAggregator = eventAggregator;
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private ITabManager TabManager { get; }

    private IAspireServiceManager AspireServiceManager { get; }

    private IEventAggregator EventAggregator { get; }

    private void OnAspireDashboardStartFaulted(AspireServiceStartFaultedAndDisposeFinishedEvent e)
    {
        CoreWebView2.NavigateToString(e.Exception.ToString());
    }

    protected override void InternalDispose()
    {
        base.InternalDispose();

        CompositeDisposable.Dispose();
    }

    private async void OnAspireDashboardReady(AspireDashboardReadyEvent e)
    {
        await NavigateAsync(e.Uri);
    }

    protected override async Task InternalWebViewHostLoadedAsync()
    {
        var statusSnapshot = AspireServiceManager.GetStatusSnapshot();
        if (statusSnapshot.Status == ServiceStatus.Running)
        {
            await NavigateAsync(statusSnapshot.DashboardUri!);
        }
        else
        {
            EventAggregator.Subscribe<AspireDashboardReadyEvent>(OnAspireDashboardReady, true)
                .DisposeWith(CompositeDisposable);

            EventAggregator.Subscribe<AspireServiceStartFaultedAndDisposeFinishedEvent>(
                    OnAspireDashboardStartFaulted, true)
                .DisposeWith(CompositeDisposable);

            await AspireServiceManager.StartServerAsync();
        }
    }

    protected override void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled = true;
        TabManager.NavigateAsync(e.Uri);
    }
}