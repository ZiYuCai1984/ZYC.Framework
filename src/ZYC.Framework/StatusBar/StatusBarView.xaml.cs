using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.StatusBar;
using ZYC.Framework.Core;

namespace ZYC.Framework.StatusBar;

[RegisterSingleInstanceAs(typeof(StatusBarView), typeof(IStatusBarView))]
internal sealed partial class StatusBarView : INotifyPropertyChanged, IDisposable, IStatusBarView
{
    public StatusBarView(
        StatusBarConfig statusBarConfig,
        IStatusBarManager statusBarManager)
    {
        StatusBarConfig = statusBarConfig;
        StatusBarManager = statusBarManager;

        InitializeComponent();

        statusBarConfig
            .ObserveProperty(nameof(StatusBarConfig.IsVisible))
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Subscribe(_ =>
            {
                OnPropertyChanged(nameof(IsStatusBarVisible));
            }).DisposeWith(CompositeDisposable);
    }

    private StatusBarConfig StatusBarConfig { get; }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private IStatusBarManager StatusBarManager { get; }

    public bool IsStatusBarVisible => StatusBarConfig.IsVisible;

    public IStatusBarItem[] LeftStatusBarItems => StatusBarManager.GetLeftItems();

    public IStatusBarItem[] RightStatusBarItems => StatusBarManager.GetRightItems();

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public double GetActualHeight()
    {
        if (!StatusBarConfig.IsVisible)
        {
            return 0;
        }

        return Height;
    }


    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}