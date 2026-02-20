using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Core.WindowTitle;

namespace ZYC.Framework.WindowTitle.BuildIn;

[RegisterSingleInstance]
internal class FullScreenTitleItem : WindowTitleItem, INotifyPropertyChanged, IDisposable
{
    public FullScreenTitleItem(
        IEventAggregator eventAggregator,
        FullScreenCommand fullScreenCommand,
        DesktopWindowState desktopWindowState) : base(null!,
        null!)
    {
        FullScreenCommand = fullScreenCommand;
        DesktopWindowState = desktopWindowState;

        eventAggregator.Subscribe<FullScreennCommandExecutedEvent>(OnFullScreenCommandExecuted)
            .DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();

    private FullScreenCommand FullScreenCommand { get; }

    private DesktopWindowState DesktopWindowState { get; }

    public override string Icon
    {
        get
        {
            if (DesktopWindowState.WindowState == WindowState.Maximized)
            {
                return "ArrowCollapse";
            }

            return "ArrowExpandAll";
        }
    }

    public override ICommand Command => FullScreenCommand;

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnFullScreenCommandExecuted(FullScreennCommandExecutedEvent e)
    {
        OnPropertyChanged(nameof(Icon));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}