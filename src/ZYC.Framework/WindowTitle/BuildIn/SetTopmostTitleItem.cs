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
internal class SetTopmostTitleItem : WindowTitleItem, INotifyPropertyChanged, IDisposable
{
    private readonly SetTopmostCommand _command;

    public SetTopmostTitleItem(
        IEventAggregator eventAggregator,
        DesktopWindowState desktopWindowState,
        SetTopmostCommand command) : base(null!, null!)
    {
        _command = command;
        DesktopWindowState = desktopWindowState;

        eventAggregator.Subscribe<SetTopmostCommandExecutedEvent>(OnSetTopmostCommandExecuted)
            .DisposeWith(CompositeDisposable);
    }


    private CompositeDisposable CompositeDisposable { get; } = new();

    private DesktopWindowState DesktopWindowState { get; }

    public override string Icon
    {
        get
        {
            if (DesktopWindowState.Topmost)
            {
                return "PinOffOutline";
            }

            return "PinOutline";
        }
    }

    public override ICommand Command => _command;

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnSetTopmostCommandExecuted(SetTopmostCommandExecutedEvent obj)
    {
        OnPropertyChanged(nameof(Icon));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}