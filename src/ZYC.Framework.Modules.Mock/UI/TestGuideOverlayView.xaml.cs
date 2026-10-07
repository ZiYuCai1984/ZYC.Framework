using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Overlay;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.Mock.UI;

[Register]
internal partial class TestGuideOverlayView : IDisposable
{
    public TestGuideOverlayView(IOverlayManager overlayManager)
    {
        OverlayManager = overlayManager;

        InitializeComponent();

        var clicks = Observable
            .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                handler => TestGuideOverlay.Click += handler,
                handler => TestGuideOverlay.Click -= handler);

        clicks
            .Take(1)
            .ObserveOnUI()
            .SelectMany(_ => Observable.FromAsync(async cancellationToken =>
            {
                using var overlay =
                    OverlayManager.Show([TextBlock, TestGuideOverlay]);

                await Task.Delay(3000, cancellationToken);
            }))
            .Repeat()
            .Subscribe(
                _ => { },
                error => Trace.TraceError(error.ToString()))
            .DisposeWith(CompositeDisposable);
    }

    private IOverlayManager OverlayManager { get; }

    private CompositeDisposable CompositeDisposable { get; } = new();


    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }
}