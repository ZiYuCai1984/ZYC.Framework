using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Mock.Abstractions;

namespace ZYC.Framework.Modules.Mock.UI;

[Register]
internal partial class TestHybridIconView : IDisposable, INotifyPropertyChanged
{
    public TestHybridIconView(TestHybridIconPageState testHybridIconPageState, IAppContext appContext)
    {
        TestHybridIconPageState = testHybridIconPageState;

        InitializeComponent();

        var throttleTimeSpan = TimeSpan.FromMilliseconds(200);

        TestHybridIconPageState.ObserveProperty(nameof(TestHybridIconPageState.IconData))
            .Throttle(throttleTimeSpan)
            .ObserveOnUI()
            .Subscribe(s => { OnPropertyChanged(nameof(IconData)); })
            .DisposeWith(CompositeDisposable);

        TestHybridIconPageState.ObserveProperty(nameof(TestHybridIconPageState.Width))
            .Throttle(throttleTimeSpan)
            .ObserveOnUI()
            .Subscribe(s => { OnPropertyChanged(nameof(IconWidth)); })
            .DisposeWith(CompositeDisposable);

        TestHybridIconPageState.ObserveProperty(nameof(TestHybridIconPageState.Height))
            .Throttle(throttleTimeSpan)
            .ObserveOnUI()
            .Subscribe(s => { OnPropertyChanged(nameof(IconHeight)); })
            .DisposeWith(CompositeDisposable);
    }

    public TestHybridIconPageState TestHybridIconPageState { get; }

    public string IconData => TestHybridIconPageState.IconData;

    public int IconWidth => TestHybridIconPageState.Width;

    public int IconHeight => TestHybridIconPageState.Height;

    private CompositeDisposable CompositeDisposable { get; } = new();

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}